// CardTextPreviewTool.cs
// 위치: Assets/Editor/CardTextPreviewTool.cs
// 사용법: 유니티 상단 메뉴 → Tools → 카드 텍스트 미리보기 (단축키: Ctrl+Shift+C)
//
// v1.4  카드 프레임 이미지(Card_Front_Adventurer) 직접 사용
//       - AssetDatabase로 Sprite 로드 → Texture2D로 변환해 배경에 표시
//       - 복잡한 더미 드로우 코드 제거, 단순화

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class CardTextPreviewTool : EditorWindow
{
    // ───────────────────────────────────────────
    // ★ 설정값
    // ───────────────────────────────────────────

    // 카드 프레임 Sprite 이름 (프로젝트 내 에셋 이름)
    private const string CARD_FRAME_SPRITE = "Card_Front_Adventurer";

    // 미리보기 표시 크기 (원본 91x120 → 약 2.6배)
    private const float PREVIEW_W = 236f;
    private const float PREVIEW_H = 312f;

    // 오버플로우 판정용 실제 카드 크기 (인스펙터 기준)
    private const float CARD_WIDTH  = 154f;
    private const float CARD_HEIGHT = 98f;
    private const int   FONT_SIZE   = 12;

    // ── 텍스트 레이아웃 (카드 이미지 기준 비율) ──
    // 이름: 상단 15.8%  (y=3~22 / 120)
    // 설명: 하단 시작점 57% (y=63~115 / 120)
    private const float NAME_Y_RATIO = 0.025f;  // 이름 Y 시작
    private const float NAME_H_RATIO = 0.140f;  // 이름 높이
    private const float DESC_Y_RATIO = 0.530f;  // 설명 Y 시작
    private const float DESC_H_RATIO = 0.430f;  // 설명 높이 (잘림 방지로 약간 늘림)
    private const float NAME_X_PAD   = 0.095f;  // 이름 좌우 패딩 비율
    private const float DESC_X_PAD   = 0.095f;  // 설명 좌우 패딩 비율

    // 텍스트 색상
    private static readonly Color COL_NAME_TEXT = new Color(0.98f, 0.98f, 1.00f);
    private static readonly Color COL_DESC_TEXT = new Color(0.92f, 0.92f, 0.88f);

    // ───────────────────────────────────────────
    // 에셋 참조
    // ───────────────────────────────────────────
    private ScriptableObject localizeTableAsset;
    private ScriptableObject toolExcelTableAsset;

    // 카드 프레임 텍스처 캐시
    private Texture2D cardFrameTex;

    // ───────────────────────────────────────────
    // 내부 데이터
    // ───────────────────────────────────────────
    private struct CardEntry
    {
        public string cardKey;
        public string nameLocaleKey;
        public string descLocaleKey;
        public bool   isDummy;
        public string dummyName;
        public string dummyDesc;
    }

    private Dictionary<string, string> localeKor = new Dictionary<string, string>();
    private Dictionary<string, string> localeEng = new Dictionary<string, string>();
    private List<CardEntry>            cards      = new List<CardEntry>();

    // UI 상태
    private Vector2 listScrollPos;
    private string  searchFilter     = "";
    private bool    showOverflowOnly = false;
    private bool    useEnglish       = false;
    private int     selectedIndex    = -1;
    private string  statusMsg        = "에셋을 연결하고 [불러오기]를 눌러주세요.";

    // 폰트 경로
    private const string FONT_BOLD_PATH = "Assets/Sprite/Galmuri11-Bold.ttf";
    private const string FONT_REG_PATH  = "Assets/Sprite/Galmuri11-Bold.ttf"; // Regular가 없으면 Bold 공용

    // 폰트 캐시
    private Font fontBold;
    private Font fontRegular;

    // 스타일 캐시
    private GUIStyle cardNameStyle;
    private GUIStyle cardDescStyle;
    private GUIStyle warnStyle;
    private bool     stylesReady = false;

    // ───────────────────────────────────────────
    // 메뉴 등록
    // ───────────────────────────────────────────
    [MenuItem("Tools/카드 텍스트 미리보기 %#c")]
    public static void ShowWindow()
    {
        var w = GetWindow<CardTextPreviewTool>("카드 텍스트 미리보기");
        w.minSize = new Vector2(980, 580);
        w.Show();
    }

    // ───────────────────────────────────────────
    // OnEnable
    // ───────────────────────────────────────────
    private void OnEnable()
    {
        LoadCardFrameTexture();
        LoadFonts();
        TryAutoFindAssets();
    }

    // ── 폰트 로드 ────────────────────────────────
    private void LoadFonts()
    {
        // 경로 직접 로드 시도
        fontBold    = AssetDatabase.LoadAssetAtPath<Font>(FONT_BOLD_PATH);
        fontRegular = AssetDatabase.LoadAssetAtPath<Font>(FONT_REG_PATH);

        // 실패 시 프로젝트 전체 탐색
        if (fontBold == null)
        {
            string[] g = AssetDatabase.FindAssets("Galmuri11-Bold t:Font");
            if (g.Length > 0)
                fontBold = AssetDatabase.LoadAssetAtPath<Font>(
                    AssetDatabase.GUIDToAssetPath(g[0]));
        }

        if (fontRegular == null)
        {
            // Regular 폰트 탐색 (Bold 제외)
            string[] g = AssetDatabase.FindAssets("Galmuri11 t:Font");
            foreach (var guid in g)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (!p.Contains("Bold"))
                { fontRegular = AssetDatabase.LoadAssetAtPath<Font>(p); break; }
            }
            // Regular 없으면 Bold로 대체
            if (fontRegular == null) fontRegular = fontBold;
        }

        // 폰트가 바뀌었으므로 스타일 재초기화
        stylesReady = false;

        if (fontBold != null)
            Debug.Log($"[카드 미리보기] Bold 폰트 로드 완료: {AssetDatabase.GetAssetPath(fontBold)}");
        else
            Debug.LogWarning("[카드 미리보기] Galmuri11-Bold 폰트를 찾을 수 없습니다. 기본 폰트를 사용합니다.");
    }

    // ── 카드 프레임 텍스처 로드 ─────────────────
    private void LoadCardFrameTexture()
    {
        cardFrameTex = null;

        // 프로젝트 전체에서 이름으로 Sprite 검색
        string[] guids = AssetDatabase.FindAssets($"{CARD_FRAME_SPRITE} t:Sprite");
        if (guids.Length == 0)
        {
            // Texture2D로도 검색
            guids = AssetDatabase.FindAssets($"{CARD_FRAME_SPRITE} t:Texture2D");
        }

        if (guids.Length == 0)
        {
            Debug.LogWarning($"[카드 미리보기] '{CARD_FRAME_SPRITE}' 스프라이트를 찾을 수 없습니다.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        // Sprite로 로드 후 Texture2D 추출
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null)
        {
            cardFrameTex = sprite.texture;
        }
        else
        {
            // Sprite가 아니면 Texture2D로 직접 로드
            cardFrameTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        if (cardFrameTex != null)
            Debug.Log($"[카드 미리보기] 카드 프레임 로드: {path}");
        else
            Debug.LogWarning($"[카드 미리보기] 텍스처 로드 실패: {path}");
    }

    // ───────────────────────────────────────────
    // 에셋 자동 탐색
    // ───────────────────────────────────────────
    private void TryAutoFindAssets()
    {
        if (localizeTableAsset == null)
        {
            string[] g = AssetDatabase.FindAssets("LocalizeTable t:ScriptableObject");
            if (g.Length > 0)
                localizeTableAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                    AssetDatabase.GUIDToAssetPath(g[0]));
        }
        if (toolExcelTableAsset == null)
        {
            string[] g = AssetDatabase.FindAssets("ToolExcelTable t:ScriptableObject");
            if (g.Length > 0)
                toolExcelTableAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                    AssetDatabase.GUIDToAssetPath(g[0]));
        }
        if (localizeTableAsset != null && toolExcelTableAsset != null)
            LoadAll();
    }

    // ───────────────────────────────────────────
    // 데이터 로드
    // ───────────────────────────────────────────
    private void LoadAll()
    {
        var dummies = cards.FindAll(c => c.isDummy);
        cards.Clear(); localeKor.Clear(); localeEng.Clear();

        if (localizeTableAsset == null || toolExcelTableAsset == null)
        { statusMsg = "⚠ 에셋을 연결해주세요."; cards.AddRange(dummies); return; }

        LoadLocalize();
        LoadCards();
        cards.AddRange(dummies);
        UpdateStatus();
        Repaint();
    }

    private void LoadLocalize()
    {
        var so   = new SerializedObject(localizeTableAsset);
        var list = so.FindProperty("localizeDatas");
        if (list == null) { Debug.LogError("[카드 미리보기] 'localizeDatas' 없음"); return; }

        for (int i = 0; i < list.arraySize; i++)
        {
            var    e   = list.GetArrayElementAtIndex(i);
            string key = e.FindPropertyRelative("key")?.stringValue ?? "";
            string kor = e.FindPropertyRelative("kor")?.stringValue ?? "";
            string eng = e.FindPropertyRelative("eng")?.stringValue ?? "";
            if (!string.IsNullOrEmpty(key))
            { localeKor[key] = ProcessText(kor); localeEng[key] = ProcessText(eng); }
        }
    }

    private void LoadCards()
    {
        var so   = new SerializedObject(toolExcelTableAsset);
        var list = so.FindProperty("_toolExcelDatas");
        if (list == null) { Debug.LogError("[카드 미리보기] '_toolExcelDatas' 없음"); return; }

        for (int i = 0; i < list.arraySize; i++)
        {
            var    e  = list.GetArrayElementAtIndex(i);
            string ck = e.FindPropertyRelative("cardKeyString")?.stringValue    ?? "";
            string nk = e.FindPropertyRelative("cardNameKeyLocal")?.stringValue ?? "";
            string dk = e.FindPropertyRelative("cardDescKeyLocal")?.stringValue ?? "";
            if (!string.IsNullOrEmpty(ck))
                cards.Add(new CardEntry { cardKey = ck, nameLocaleKey = nk, descLocaleKey = dk });
        }
    }

    private void UpdateStatus()
    {
        int real  = cards.FindAll(c => !c.isDummy).Count;
        int dummy = cards.FindAll(c =>  c.isDummy).Count;
        statusMsg = $"카드 {real}장 로드  |  더미 {dummy}장  |  오버플로우 {CountOverflow()}장";
    }

    // ───────────────────────────────────────────
    // GUI 메인
    // ───────────────────────────────────────────
    private void OnGUI()
    {
        InitStyles();
        DrawAssetSlots();
        DrawToolbar();
        EditorGUILayout.BeginHorizontal();
        DrawCardList();
        DrawPreviewPanel();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField(statusMsg, EditorStyles.centeredGreyMiniLabel);
    }

    // ── 에셋 슬롯 ────────────────────────────────
    private void DrawAssetSlots()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("에셋 연결", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        localizeTableAsset  = (ScriptableObject)EditorGUILayout.ObjectField(
            "LocalizeTable",  localizeTableAsset,  typeof(ScriptableObject), false);
        toolExcelTableAsset = (ScriptableObject)EditorGUILayout.ObjectField(
            "ToolExcelTable", toolExcelTableAsset, typeof(ScriptableObject), false);
        if (EditorGUI.EndChangeCheck() && localizeTableAsset != null && toolExcelTableAsset != null)
            LoadAll();
        EditorGUILayout.EndVertical();
    }

    // ── 툴바 ────────────────────────────────────
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("🔄 새로고침", EditorStyles.toolbarButton, GUILayout.Width(90)))
            LoadAll();
        if (GUILayout.Button("🖼 프레임 재로드", EditorStyles.toolbarButton, GUILayout.Width(100)))
            LoadCardFrameTexture();
        if (GUILayout.Button("🔤 폰트 재로드", EditorStyles.toolbarButton, GUILayout.Width(95)))
            LoadFonts();

        GUILayout.Space(8);
        GUILayout.Label("검색:", GUILayout.Width(34));
        searchFilter = EditorGUILayout.TextField(searchFilter,
            EditorStyles.toolbarSearchField, GUILayout.Width(160));
        GUILayout.Space(8);
        showOverflowOnly = GUILayout.Toggle(showOverflowOnly,
            "⚠ 오버플로우만", EditorStyles.toolbarButton, GUILayout.Width(100));
        GUILayout.Space(8);
        useEnglish = GUILayout.Toggle(useEnglish,
            useEnglish ? "EN" : "KO", EditorStyles.toolbarButton, GUILayout.Width(38));
        GUILayout.Space(8);
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.5f);
        if (GUILayout.Button("＋ 더미 카드", EditorStyles.toolbarButton, GUILayout.Width(90)))
            AddDummyCard();
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();

        // 프레임 로드 상태 표시
        if (cardFrameTex == null)
        {
            GUI.color = new Color(1f, 0.5f, 0.3f);
            GUILayout.Label("⚠ 프레임 없음", EditorStyles.miniLabel, GUILayout.Width(80));
            GUI.color = Color.white;
        }

        int oc = CountOverflow();
        GUI.color = oc > 0 ? new Color(1f, 0.7f, 0.2f) : new Color(0.45f, 1f, 0.45f);
        GUILayout.Label($"⚠ {oc} / {cards.Count}장",
            EditorStyles.toolbarButton, GUILayout.Width(100));
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    // ── 카드 목록 ────────────────────────────────
    private void DrawCardList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(260), GUILayout.ExpandHeight(true));
        GUILayout.Label("카드 목록", EditorStyles.boldLabel);
        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, GUILayout.ExpandHeight(true));

        for (int i = 0; i < cards.Count; i++)
        {
            var    c       = cards[i];
            string name    = GetCardName(c);
            bool   overflow = IsOverflow(StripTmpTags(GetCardDesc(c)));

            if (!string.IsNullOrEmpty(searchFilter) &&
                !name.Contains(searchFilter) && !c.cardKey.Contains(searchFilter)) continue;
            if (showOverflowOnly && !overflow) continue;

            EditorGUILayout.BeginHorizontal();
            GUI.color = overflow ? new Color(1f, 0.55f, 0.1f) : new Color(0.5f, 1f, 0.5f);
            GUILayout.Label(overflow ? "⚠" : "✓", GUILayout.Width(16));
            GUI.color = Color.white;

            if (c.isDummy)
            {
                GUI.color = new Color(0.4f, 0.9f, 0.6f);
                GUILayout.Label("[D]", GUILayout.Width(22));
                GUI.color = Color.white;
            }

            GUI.backgroundColor = selectedIndex == i ? new Color(0.35f, 0.55f, 1f) : Color.white;
            if (GUILayout.Button(c.isDummy ? $"[더미] {name}" : name, EditorStyles.miniButton))
                selectedIndex = i;
            GUI.backgroundColor = Color.white;

            if (c.isDummy)
            {
                GUI.color = new Color(1f, 0.4f, 0.4f);
                if (GUILayout.Button("✕", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    cards.RemoveAt(i);
                    if (selectedIndex >= cards.Count) selectedIndex = cards.Count - 1;
                    UpdateStatus(); GUIUtility.ExitGUI();
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    // ── 미리보기 패널 ────────────────────────────
    private void DrawPreviewPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        if (selectedIndex < 0 || selectedIndex >= cards.Count)
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("← 카드를 선택하세요", EditorStyles.centeredGreyMiniLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            return;
        }

        var card = cards[selectedIndex];
        GUILayout.Label("미리보기", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        DrawCardVisual(card);
        if (card.isDummy) DrawDummyEditPanel(selectedIndex);
        else              DrawInfoPanel(card);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    // ── 카드 비주얼 (이미지 배경 + 텍스트 오버레이) ──
    private void DrawCardVisual(CardEntry card)
    {
        string name     = GetCardName(card);
        string clean    = StripTmpTags(GetCardDesc(card));
        bool   overflow = IsOverflow(clean);

        float pw = PREVIEW_W;
        float ph = PREVIEW_H;

        Rect area = GUILayoutUtility.GetRect(pw + 24, ph + 24);
        float cx = area.x + 12;
        float cy = area.y + 12;
        Rect cardRect = new Rect(cx, cy, pw, ph);

        // ① 카드 프레임 이미지 배경
        if (cardFrameTex != null)
        {
            // Point 필터로 픽셀 아트 선명하게 표시
            GUI.DrawTexture(cardRect, cardFrameTex, ScaleMode.StretchToFill, true);
        }
        else
        {
            // 프레임 없을 때 폴백 배경
            EditorGUI.DrawRect(cardRect, new Color(0.10f, 0.10f, 0.28f));
            DrawBorder(cardRect, new Color(0.36f, 0.60f, 0.86f), 2f);
            GUI.color = new Color(1f, 0.5f, 0.3f);
            GUI.Label(cardRect, $"'{CARD_FRAME_SPRITE}'\n스프라이트를 찾을 수 없습니다.\n\n툴바 [🖼 프레임 재로드] 클릭",
                new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, wordWrap = true, fontSize = 10 });
            GUI.color = Color.white;
        }

        // ② 이름 텍스트 오버레이
        float nameX = cx + pw * NAME_X_PAD;
        float nameY = cy + ph * NAME_Y_RATIO;
        float nameW = pw * (1f - NAME_X_PAD * 2f);
        float nameH = ph * NAME_H_RATIO;
        GUI.Label(new Rect(nameX, nameY, nameW, nameH), name, cardNameStyle);

        // ③ 설명 텍스트 오버레이 (TMP 색상 태그 파싱 적용)
        float descX = cx + pw * DESC_X_PAD;
        float descY = cy + ph * DESC_Y_RATIO;
        float descW = pw * (1f - DESC_X_PAD * 2f);
        float descH = ph * DESC_H_RATIO;

        var segments = ParseTmpSegments(GetCardDesc(card));
        DrawTmpSegments(segments, new Rect(descX, descY, descW, descH), cardDescStyle);

        // ④ 오버플로우 경고 오버레이
        if (overflow)
        {
            EditorGUI.DrawRect(new Rect(cx, cy + ph - 24, pw, 24),
                               new Color(1f, 0.25f, 0f, 0.88f));
            GUI.Label(new Rect(cx, cy + ph - 24, pw, 24),
                      "⚠ 텍스트 오버플로우!", warnStyle);
        }
    }

    // ── 일반 카드 정보 패널 ──────────────────────
    private void DrawInfoPanel(CardEntry card)
    {
        string name     = GetCardName(card);
        string rawDesc  = GetCardDesc(card);
        string clean    = StripTmpTags(rawDesc);
        bool   overflow = IsOverflow(clean);

        EditorGUILayout.BeginVertical(GUILayout.Width(330));
        EditorGUILayout.LabelField($"Key: {card.cardKey}", EditorStyles.miniLabel);
        GUILayout.Space(6);

        GUILayout.Label("카드 이름 키", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(card.nameLocaleKey,
            EditorStyles.miniTextField, GUILayout.Height(18));
        EditorGUILayout.LabelField($"→  {name}", EditorStyles.miniLabel);

        GUILayout.Space(6);
        GUILayout.Label("설명 키", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(card.descLocaleKey,
            EditorStyles.miniTextField, GUILayout.Height(18));

        GUILayout.Space(4);
        GUILayout.Label("원본 (TMP 태그 포함)", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(rawDesc, EditorStyles.textArea, GUILayout.Height(64));

        GUILayout.Space(4);
        GUILayout.Label("렌더 텍스트 (태그 제거)", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(clean, EditorStyles.textArea, GUILayout.Height(48));

        GUILayout.Space(6);
        int lc = clean.Split('\n').Length;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"글자 수: {clean.Length}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"줄 수: {lc}",            EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(4);
        DrawOverflowResult(overflow);
        EditorGUILayout.EndVertical();
    }

    // ── 더미 카드 편집 패널 ──────────────────────
    private void DrawDummyEditPanel(int idx)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(330));
        GUI.color = new Color(0.5f, 1f, 0.7f);
        GUILayout.Label("✏ 더미 카드 편집", EditorStyles.boldLabel);
        GUI.color = Color.white;

        GUILayout.Space(6);
        GUILayout.Label("카드 이름", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        string newName = EditorGUILayout.TextField(cards[idx].dummyName);
        if (EditorGUI.EndChangeCheck())
        { var c = cards[idx]; c.dummyName = newName; cards[idx] = c; Repaint(); }

        GUILayout.Space(6);
        GUILayout.Label("설명 텍스트", EditorStyles.boldLabel);
        GUILayout.Label("(TMP 태그 사용 가능: <#fbc531>■</color>, <br>)", EditorStyles.miniLabel);
        EditorGUI.BeginChangeCheck();
        string newDesc = EditorGUILayout.TextArea(cards[idx].dummyDesc, GUILayout.Height(80));
        if (EditorGUI.EndChangeCheck())
        { var c = cards[idx]; c.dummyDesc = newDesc; cards[idx] = c; UpdateStatus(); Repaint(); }

        GUILayout.Space(6);
        string clean    = StripTmpTags(cards[idx].dummyDesc);
        bool   overflow = IsOverflow(clean);
        int    lc       = clean.Split('\n').Length;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"글자 수: {clean.Length}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"줄 수: {lc}",            EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(4);
        DrawOverflowResult(overflow);

        GUILayout.Space(8);
        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("이 더미 카드 삭제"))
        {
            cards.RemoveAt(idx);
            selectedIndex = Mathf.Clamp(idx - 1, 0, cards.Count - 1);
            UpdateStatus(); GUIUtility.ExitGUI();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();
    }

    // ── 오버플로우 결과 ──────────────────────────
    private void DrawOverflowResult(bool overflow)
    {
        if (overflow)
        {
            GUI.color = new Color(1f, 0.55f, 0.1f);
            GUILayout.Label("⚠  텍스트가 카드 영역을 벗어납니다!", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.LabelField(
                $"카드 영역: {CARD_WIDTH}×{CARD_HEIGHT}px (오버플로우 판정 기준)",
                EditorStyles.miniLabel);
        }
        else
        {
            GUI.color = new Color(0.4f, 1f, 0.4f);
            GUILayout.Label("✓  텍스트가 카드 영역 안에 들어옵니다.", EditorStyles.boldLabel);
            GUI.color = Color.white;
        }
    }

    // ───────────────────────────────────────────
    // 더미 카드 추가
    // ───────────────────────────────────────────
    private void AddDummyCard()
    {
        int n = cards.FindAll(c => c.isDummy).Count + 1;
        cards.Add(new CardEntry
        {
            cardKey   = $"Dummy_{n}",
            isDummy   = true,
            dummyName = $"더미 카드 {n}",
            dummyDesc = "여기에 설명을 입력하세요."
        });
        selectedIndex = cards.Count - 1;
        UpdateStatus(); Repaint();
    }

    // ───────────────────────────────────────────
    // 유틸리티
    // ───────────────────────────────────────────
    private string GetCardName(CardEntry c)
        => c.isDummy ? c.dummyName : GetLocaleText(c.nameLocaleKey);

    private string GetCardDesc(CardEntry c)
        => c.isDummy ? c.dummyDesc : GetLocaleText(c.descLocaleKey);

    // TMP 텍스트 세그먼트 (색상 + 텍스트)
    private struct TextSegment
    {
        public string text;
        public Color  color;
    }

    // TMP 태그를 파싱해 색상별 세그먼트 리스트로 변환
    private List<TextSegment> ParseTmpSegments(string raw)
    {
        var result  = new List<TextSegment>();
        if (string.IsNullOrEmpty(raw)) return result;

        // \\n → 줄바꿈, <br> → 줄바꿈 먼저 처리
        raw = raw.Replace("\\n", "\n");
        raw = Regex.Replace(raw, @"<br\s*/?>", "\n");

        // 색상 태그 파싱: <#rrggbb>text</color>
        var    pattern     = new Regex(@"<#([0-9a-fA-F]{6})>(.*?)</color>", RegexOptions.Singleline);
        int    lastIndex   = 0;
        Color  defaultCol  = COL_DESC_TEXT;

        foreach (Match m in pattern.Matches(raw))
        {
            // 태그 앞의 일반 텍스트
            if (m.Index > lastIndex)
            {
                string plain = raw.Substring(lastIndex, m.Index - lastIndex);
                // 나머지 태그 제거
                plain = Regex.Replace(plain, @"<[^>]+>", "");
                if (!string.IsNullOrEmpty(plain))
                    result.Add(new TextSegment { text = plain, color = defaultCol });
            }

            // 색상 태그 내 텍스트
            string hex     = m.Groups[1].Value;
            string content = m.Groups[2].Value;
            content = Regex.Replace(content, @"<[^>]+>", ""); // 중첩 태그 제거
            if (!string.IsNullOrEmpty(content))
            {
                Color col = defaultCol;
                ColorUtility.TryParseHtmlString($"#{hex}", out col);
                result.Add(new TextSegment { text = content, color = col });
            }

            lastIndex = m.Index + m.Length;
        }

        // 마지막 남은 일반 텍스트
        if (lastIndex < raw.Length)
        {
            string tail = raw.Substring(lastIndex);
            tail = Regex.Replace(tail, @"<[^>]+>", "");
            if (!string.IsNullOrEmpty(tail))
                result.Add(new TextSegment { text = tail, color = defaultCol });
        }

        // 색상 태그가 전혀 없으면 전체를 기본 색상으로
        if (result.Count == 0)
            result.Add(new TextSegment { text = Regex.Replace(raw, @"<[^>]+>", ""), color = defaultCol });

        return result;
    }

    // ★ 특수기호 너비 등록 (새 기호 추가 시 여기에만 추가)
    private static readonly Dictionary<char, float> SPECIAL_CHAR_WIDTHS = new Dictionary<char, float>
    {
        { '■', 11.5f },
        { '↖', 11.5f },
        { '↗', 11.5f },
        { '↙', 11.5f },
        { '↘', 11.5f },
        { '←', 11.5f },
        { '↑', 11.5f },
        { '↓', 11.5f },
        { '→', 11.5f },
    };

    private float GetCharWidth(char c, GUIStyle baseStyle)
    {
        if (c >= 0xAC00 && c <= 0xD7A3) return 13f;  // 한글 음절
        if (c >= 0x3130 && c <= 0x318F) return 13f;  // 한글 자모
        if (c >= 0x0020 && c <= 0x007E) return 7.3f; // 영문/숫자/기본기호(공백 포함)
        if (SPECIAL_CHAR_WIDTHS.TryGetValue(c, out float w)) return w;
        return 13f; // 등록 안 된 특수기호 기본값
    }

    // TMP 세그먼트를 카드 설명 영역에 렌더링 (글자 단위 자동 줄바꿈)
    private void DrawTmpSegments(List<TextSegment> segments, Rect area, GUIStyle baseStyle)
    {
        if (segments == null || segments.Count == 0) return;

        float lineH = baseStyle.fontSize * 1.6f;

        // Step 1: 글자 단위로 줄 분리 (CalcSize 기반 정확한 너비 측정)
        var   lineList = new List<List<(char, Color)>>();
        var   curLine  = new List<(char, Color)>();
        float curW     = 0f;

        foreach (var seg in segments)
        {
            string[] parts = seg.text.Split('\n');
            for (int pi = 0; pi < parts.Length; pi++)
            {
                if (pi > 0)
                {
                    lineList.Add(new List<(char, Color)>(curLine));
                    curLine.Clear();
                    curW = 0f;
                }
                foreach (char c in parts[pi])
                {
                    float cw = GetCharWidth(c, baseStyle);
                    if (curW + cw > area.width && curLine.Count > 0)
                    {
                        lineList.Add(new List<(char, Color)>(curLine));
                        curLine.Clear();
                        curW = 0f;
                    }
                    curLine.Add((c, seg.color));
                    curW += cw;
                }
            }
        }
        if (curLine.Count > 0)
            lineList.Add(new List<(char, Color)>(curLine));

        // Step 2: 줄별 렌더링
        GUI.BeginClip(new Rect(area.x, area.y, area.width, area.height));
        float y = 0f;
        foreach (var line in lineList)
        {
            float x = 0f;
            int   i = 0;
            while (i < line.Count)
            {
                Color groupCol = line[i].Item2;
                int   j        = i;
                while (j < line.Count && line[j].Item2 == groupCol) j++;

                var sb = new System.Text.StringBuilder();
                for (int k = i; k < j; k++) sb.Append(line[k].Item1);
                string token = sb.ToString();

                var style = new GUIStyle(baseStyle)
                {
                    wordWrap = false,
                    clipping = TextClipping.Clip,
                    normal   = { textColor = groupCol }
                };

                // 토큰 너비 = 글자별 너비 합산
                float w = 0f;
                foreach (char c in token) w += GetCharWidth(c, baseStyle);

                GUI.Label(new Rect(x, y, w + 2f, lineH), token, style);
                x += w;
                i  = j;
            }
            y += lineH;
        }
        GUI.EndClip();
    }

    private string ProcessText(string text)
        => string.IsNullOrEmpty(text) ? text : text.Replace("\\n", "\n").Trim();

    // 오버플로우 판정 및 렌더 텍스트 패널용 태그 제거
    private string StripTmpTags(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        text = Regex.Replace(text, @"<#[0-9a-fA-F]{6}>", "");
        text = Regex.Replace(text, @"</color>", "");
        text = Regex.Replace(text, @"<br\s*/?>", "\n");
        text = Regex.Replace(text, @"<[^>]+>", "");
        text = text.Replace("\\n", "\n");
        return text.Trim();
    }

    private bool IsOverflow(string clean)
    {
        if (string.IsNullOrEmpty(clean)) return false;
        float charW    = FONT_SIZE * 0.65f;
        int   perLine  = Mathf.FloorToInt(CARD_WIDTH / charW);
        float lineH    = FONT_SIZE * 1.25f;
        int   maxLines = Mathf.FloorToInt(CARD_HEIGHT / lineH);
        int   total    = 0;
        foreach (var line in clean.Split('\n'))
            total += Mathf.Max(1, Mathf.CeilToInt((float)line.Length / perLine));
        return total > maxLines;
    }

    private int CountOverflow()
    {
        int n = 0;
        foreach (var c in cards)
            if (IsOverflow(StripTmpTags(GetCardDesc(c)))) n++;
        return n;
    }

    private string GetLocaleText(string key)
    {
        var dict = useEnglish ? localeEng : localeKor;
        return dict.TryGetValue(key, out string v) ? v : $"[{key}]";
    }

    private void DrawBorder(Rect r, Color col, float t)
    {
        EditorGUI.DrawRect(new Rect(r.x,        r.y,        r.width, t),        col);
        EditorGUI.DrawRect(new Rect(r.x,        r.yMax - t, r.width, t),        col);
        EditorGUI.DrawRect(new Rect(r.x,        r.y,        t,       r.height), col);
        EditorGUI.DrawRect(new Rect(r.xMax - t, r.y,        t,       r.height), col);
    }

    private void InitStyles()
    {
        if (stylesReady) return;

        cardNameStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = FONT_SIZE,
            fontStyle = FontStyle.Normal,   // TTF Bold 파일이므로 Normal
            alignment = TextAnchor.MiddleLeft,
            wordWrap  = false,
            font      = fontBold,           // Galmuri11-Bold.ttf
            normal    = { textColor = COL_NAME_TEXT }
        };

        cardDescStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = FONT_SIZE,
            fontStyle = FontStyle.Normal,
            wordWrap  = true,
            font      = fontRegular,  // Galmuri11 Regular (없으면 Bold 대체)
            normal    = { textColor = COL_DESC_TEXT }
        };

        warnStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize  = 11,
            normal    = { textColor = Color.white }
        };

        stylesReady = true;
    }
}