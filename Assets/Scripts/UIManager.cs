using UnityEngine;
using UnityEngine.UIElements;

namespace QonversionSample
{
    /// <summary>
    /// Manages UI navigation and screen switching using UI Toolkit.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private AppState _appState;
        private VisualElement _root;
        private VisualElement _screenContainer;
        private VisualElement _loadingOverlay;
        private Label _messageLabel;

        // Screen controllers
        private MainScreenController _mainScreen;
        private ProductsScreenController _productsScreen;
        private EntitlementsScreenController _entitlementsScreen;
        private OfferingsScreenController _offeringsScreen;
        private RemoteConfigsScreenController _remoteConfigsScreen;
        private UserScreenController _userScreen;
        private OtherScreenController _otherScreen;
        private NoCodesScreenController _noCodesScreen;

        private BaseScreenController _currentScreen;

        public void Initialize(AppState appState)
        {
            _appState = appState;
            _appState.OnStateChanged += OnStateChanged;
            _appState.OnError += ShowError;
            _appState.OnSuccess += ShowSuccess;
        }

        private void Start()
        {
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument is not assigned!");
                return;
            }

            if (_appState == null)
            {
                Debug.LogError("AppState is not initialized! Make sure SampleApp.Initialize is called in Awake.");
                return;
            }

            _root = uiDocument.rootVisualElement;
            SetupUI();
        }

        private void SetupUI()
        {
            // Clear existing content
            _root.Clear();

            // Modern dark background
            _root.style.backgroundColor = new Color(0.06f, 0.06f, 0.08f, 1f);

            // Create safe area container for mobile devices
            var safeArea = new VisualElement();
            safeArea.style.flexGrow = 1;
            safeArea.style.flexDirection = FlexDirection.Column;
            
            // Add safe area padding for notches/status bars
            var safeAreaPadding = GetSafeAreaPadding();
            safeArea.style.paddingTop = safeAreaPadding.top;
            safeArea.style.paddingBottom = safeAreaPadding.bottom;
            safeArea.style.paddingLeft = safeAreaPadding.left;
            safeArea.style.paddingRight = safeAreaPadding.right;
            _root.Add(safeArea);

            // Create main container
            var mainContainer = new VisualElement();
            mainContainer.style.flexGrow = 1;
            mainContainer.style.flexDirection = FlexDirection.Column;
            safeArea.Add(mainContainer);

            // Create screen container with scroll (now at top)
            var screenScroll = new ScrollView();
            screenScroll.style.flexGrow = 1;
            screenScroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            screenScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            screenScroll.mode = ScrollViewMode.Vertical;
            
            _screenContainer = new VisualElement();
            _screenContainer.style.paddingTop = 8;
            _screenContainer.style.paddingBottom = 8;
            _screenContainer.style.paddingLeft = 8;
            _screenContainer.style.paddingRight = 8;
            _screenContainer.style.flexShrink = 1;
            _screenContainer.style.overflow = Overflow.Hidden;
            screenScroll.Add(_screenContainer);
            mainContainer.Add(screenScroll);

            // Create bottom navigation bar
            var bottomNav = CreateBottomNavigation();
            mainContainer.Add(bottomNav);

            // Create message label
            _messageLabel = new Label();
            _messageLabel.style.position = Position.Absolute;
            _messageLabel.style.bottom = 60;
            _messageLabel.style.left = 8;
            _messageLabel.style.right = 8;
            _messageLabel.style.paddingTop = 6;
            _messageLabel.style.paddingBottom = 6;
            _messageLabel.style.paddingLeft = 10;
            _messageLabel.style.paddingRight = 10;
            _messageLabel.style.borderTopLeftRadius = 6;
            _messageLabel.style.borderTopRightRadius = 6;
            _messageLabel.style.borderBottomLeftRadius = 6;
            _messageLabel.style.borderBottomRightRadius = 6;
            _messageLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _messageLabel.style.fontSize = 10;
            _messageLabel.style.display = DisplayStyle.None;
            _root.Add(_messageLabel);

            // Create loading overlay
            _loadingOverlay = new VisualElement();
            _loadingOverlay.style.position = Position.Absolute;
            _loadingOverlay.style.top = 0;
            _loadingOverlay.style.bottom = 0;
            _loadingOverlay.style.left = 0;
            _loadingOverlay.style.right = 0;
            _loadingOverlay.style.backgroundColor = new Color(0, 0, 0, 0.5f);
            _loadingOverlay.style.justifyContent = Justify.Center;
            _loadingOverlay.style.alignItems = Align.Center;
            _loadingOverlay.style.display = DisplayStyle.None;

            var loadingLabel = new Label("Loading...");
            loadingLabel.style.color = Color.white;
            loadingLabel.style.fontSize = 14;
            loadingLabel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            loadingLabel.style.paddingTop = 10;
            loadingLabel.style.paddingBottom = 10;
            loadingLabel.style.paddingLeft = 20;
            loadingLabel.style.paddingRight = 20;
            loadingLabel.style.borderTopLeftRadius = 8;
            loadingLabel.style.borderTopRightRadius = 8;
            loadingLabel.style.borderBottomLeftRadius = 8;
            loadingLabel.style.borderBottomRightRadius = 8;
            _loadingOverlay.Add(loadingLabel);
            _root.Add(_loadingOverlay);

            // Initialize screen controllers
            InitializeScreenControllers();

            // Show main screen by default
            ShowScreen(_mainScreen);
        }

        private VisualElement CreateBottomNavigation()
        {
            // Scrollable bottom navigation for mobile
            var navScroll = new ScrollView(ScrollViewMode.Horizontal);
            navScroll.style.flexShrink = 0;
            navScroll.style.backgroundColor = new Color(0.08f, 0.08f, 0.1f, 1f);
            navScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            navScroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            navScroll.style.borderTopWidth = 1;
            navScroll.style.borderTopColor = new Color(0.15f, 0.15f, 0.18f, 1f);

            var nav = new VisualElement();
            nav.style.flexDirection = FlexDirection.Row;
            nav.style.paddingTop = 4;
            nav.style.paddingBottom = 4;
            nav.style.paddingLeft = 4;
            nav.style.paddingRight = 4;

            var buttons = new (string label, System.Action action)[]
            {
                ("Home", () => ShowScreen(_mainScreen)),
                ("Prod", () => ShowScreen(_productsScreen)),
                ("Ent", () => ShowScreen(_entitlementsScreen)),
                ("Offer", () => ShowScreen(_offeringsScreen)),
                ("Cfg", () => ShowScreen(_remoteConfigsScreen)),
                ("User", () => ShowScreen(_userScreen)),
                ("NoCodes", () => ShowScreen(_noCodesScreen)),
                ("Other", () => ShowScreen(_otherScreen))
            };

            foreach (var (label, action) in buttons)
            {
                var button = new Button(action) { text = label };
                button.style.marginRight = 2;
                button.style.marginLeft = 2;
                button.style.paddingTop = 6;
                button.style.paddingBottom = 6;
                button.style.paddingLeft = 8;
                button.style.paddingRight = 8;
                button.style.backgroundColor = new Color(0.12f, 0.12f, 0.14f, 1f);
                button.style.borderTopLeftRadius = 4;
                button.style.borderTopRightRadius = 4;
                button.style.borderBottomLeftRadius = 4;
                button.style.borderBottomRightRadius = 4;
                button.style.borderTopWidth = 0;
                button.style.borderBottomWidth = 0;
                button.style.borderLeftWidth = 0;
                button.style.borderRightWidth = 0;
                button.style.fontSize = 9;
                button.style.color = new Color(0.8f, 0.8f, 0.85f, 1f);

                nav.Add(button);
            }

            navScroll.Add(nav);
            return navScroll;
        }

        private void InitializeScreenControllers()
        {
            _mainScreen = new MainScreenController(_appState);
            _productsScreen = new ProductsScreenController(_appState);
            _entitlementsScreen = new EntitlementsScreenController(_appState);
            _offeringsScreen = new OfferingsScreenController(_appState);
            _remoteConfigsScreen = new RemoteConfigsScreenController(_appState);
            _userScreen = new UserScreenController(_appState);
            _otherScreen = new OtherScreenController(_appState);
            _noCodesScreen = new NoCodesScreenController(_appState);
        }

        private void ShowScreen(BaseScreenController screen)
        {
            _currentScreen?.OnHide();
            _screenContainer.Clear();
            _currentScreen = screen;

            var screenElement = screen.CreateUI();
            _screenContainer.Add(screenElement);
            screen.OnShow();
        }

        private void OnStateChanged()
        {
            _loadingOverlay.style.display = _appState.IsLoading ? DisplayStyle.Flex : DisplayStyle.None;
            _currentScreen?.Refresh();
        }

        private void ShowError(string message)
        {
            ShowMessage(message, new Color(0.8f, 0.2f, 0.2f, 1f));
        }

        private void ShowSuccess(string message)
        {
            ShowMessage(message, new Color(0.2f, 0.7f, 0.3f, 1f));
        }

        private void ShowMessage(string message, Color backgroundColor)
        {
            _messageLabel.text = message;
            _messageLabel.style.backgroundColor = backgroundColor;
            _messageLabel.style.color = Color.white;
            _messageLabel.style.display = DisplayStyle.Flex;

            // Hide after 3 seconds
            StartCoroutine(HideMessageAfterDelay(3f));
        }

        private System.Collections.IEnumerator HideMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _messageLabel.style.display = DisplayStyle.None;
        }

        private void OnDisable()
        {
            if (_appState != null)
            {
                _appState.OnStateChanged -= OnStateChanged;
                _appState.OnError -= ShowError;
                _appState.OnSuccess -= ShowSuccess;
            }
        }

        private (float top, float bottom, float left, float right) GetSafeAreaPadding()
        {
            var safeArea = Screen.safeArea;
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            // Calculate padding from safe area
            float top = screenHeight - (safeArea.y + safeArea.height);
            float bottom = safeArea.y;
            float left = safeArea.x;
            float right = screenWidth - (safeArea.x + safeArea.width);

            // Convert to UI Toolkit units (approximate)
            // Add minimum padding for comfortable touch
            top = Mathf.Max(top / 2f, 8f);
            bottom = Mathf.Max(bottom / 2f, 8f);
            left = Mathf.Max(left / 2f, 4f);
            right = Mathf.Max(right / 2f, 4f);

            return (top, bottom, left, right);
        }
    }
}
