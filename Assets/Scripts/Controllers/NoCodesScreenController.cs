using System;
using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the No-Codes screen.
    /// Allows testing No-Codes SDK functionality.
    /// </summary>
    public class NoCodesScreenController : BaseScreenController
    {
        private const string DefaultContextKey = "test_screen";

        private TextField _contextKeyField;
        private TextField _localeField;
        private DropdownField _themeDropdown;
        private Label _purchaseDelegateStatusLabel;
        private Label _noCodesStatusLabel;
        private VisualElement _eventsContainer;
        private DropdownField _presentationStyleDropdown;
        private Toggle _animatedToggle;
        private Button _enablePurchaseDelegateButton;

        public NoCodesScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = CreateScrollView();

            var content = new VisualElement();
            content.style.paddingTop = 4;

            // Header
            var title = new Label("No-Codes");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            title.style.marginBottom = 6;
            content.Add(title);

            // Status Section
            var statusSection = CreateSection("Status");
            
            var statusRow = new VisualElement();
            statusRow.style.flexDirection = FlexDirection.Row;
            statusRow.style.alignItems = Align.Center;
            
            var statusLabel = new Label("No-Codes:");
            statusLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            statusLabel.style.fontSize = 9;
            statusLabel.style.marginRight = 8;
            statusRow.Add(statusLabel);
            
            _noCodesStatusLabel = new Label("Not initialized");
            _noCodesStatusLabel.style.fontSize = 9;
            statusRow.Add(_noCodesStatusLabel);
            
            statusSection.Add(statusRow);
            content.Add(statusSection);

            // Show Screen Section
            var showScreenSection = CreateSection("Show Screen");
            
            showScreenSection.Add(CreateLabeledTextField("Context Key:", DefaultContextKey, out _contextKeyField));
            
            var showScreenButton = CreateButton("Show No-Code Screen", ShowScreen);
            showScreenButton.style.marginTop = 6;
            showScreenSection.Add(showScreenButton);
            
            content.Add(showScreenSection);

            // Purchase Delegate Section
            var purchaseDelegateSection = CreateSection("Purchase Delegate");
            
            var purchaseDelegateRow = new VisualElement();
            purchaseDelegateRow.style.flexDirection = FlexDirection.Row;
            purchaseDelegateRow.style.alignItems = Align.Center;
            purchaseDelegateRow.style.marginBottom = 6;
            
            var purchaseDelegateLabel = new Label("Status:");
            purchaseDelegateLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            purchaseDelegateLabel.style.fontSize = 9;
            purchaseDelegateLabel.style.marginRight = 8;
            purchaseDelegateRow.Add(purchaseDelegateLabel);
            
            _purchaseDelegateStatusLabel = new Label("Disabled");
            _purchaseDelegateStatusLabel.style.fontSize = 9;
            purchaseDelegateRow.Add(_purchaseDelegateStatusLabel);
            
            purchaseDelegateSection.Add(purchaseDelegateRow);
            
            _enablePurchaseDelegateButton = CreateButton("Enable Custom Purchase Delegate", EnablePurchaseDelegate);
            _enablePurchaseDelegateButton.style.marginBottom = 6;
            _enablePurchaseDelegateButton.style.backgroundColor = new Color(0.2f, 0.6f, 0.3f, 1f);
            purchaseDelegateSection.Add(_enablePurchaseDelegateButton);
            
            purchaseDelegateSection.Add(CreatePlatformNote(
                "When enabled, purchases from No-Code screens will be handled by a custom delegate " +
                "that logs events and uses Qonversion SDK for the actual purchase."));
            
            content.Add(purchaseDelegateSection);

            // Presentation Config Section
            var presentationSection = CreateSection("Presentation Config");
            
            // Presentation Style Dropdown
            var styleRow = new VisualElement();
            styleRow.style.flexDirection = FlexDirection.Row;
            styleRow.style.alignItems = Align.Center;
            styleRow.style.marginBottom = 6;
            
            var styleLabel = new Label("Style:");
            styleLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            styleLabel.style.fontSize = 9;
            styleLabel.style.width = 70;
            styleRow.Add(styleLabel);
            
            _presentationStyleDropdown = new DropdownField();
            _presentationStyleDropdown.choices = new System.Collections.Generic.List<string>
            {
                "Push", "FullScreen", "Popover", "NoAnimation"
            };
            _presentationStyleDropdown.value = "FullScreen";
            _presentationStyleDropdown.style.flexGrow = 1;
            _presentationStyleDropdown.style.fontSize = 9;
            styleRow.Add(_presentationStyleDropdown);
            
            presentationSection.Add(styleRow);
            
            // Animated Toggle (iOS only)
            var animatedRow = new VisualElement();
            animatedRow.style.flexDirection = FlexDirection.Row;
            animatedRow.style.alignItems = Align.Center;
            animatedRow.style.marginBottom = 6;
            
            _animatedToggle = new Toggle("Animated (iOS only)");
            _animatedToggle.value = true;
            _animatedToggle.style.fontSize = 9;
            animatedRow.Add(_animatedToggle);
            
            presentationSection.Add(animatedRow);
            
            var setPresentationButton = CreateButton("Set Presentation Config", SetPresentationConfig, false);
            presentationSection.Add(setPresentationButton);
            
            content.Add(presentationSection);

            // Locale Section
            var localeSection = CreateSection("Locale");
            
            localeSection.Add(CreateLabeledTextField("Locale:", "en", out _localeField));
            
            var localeButtonsRow = new VisualElement();
            localeButtonsRow.style.flexDirection = FlexDirection.Row;
            localeButtonsRow.style.marginTop = 6;
            
            var setLocaleButton = CreateButton("Set Locale", SetLocale);
            setLocaleButton.style.flexGrow = 1;
            setLocaleButton.style.marginRight = 4;
            localeButtonsRow.Add(setLocaleButton);
            
            var resetLocaleButton = CreateButton("Reset", ResetLocale, false);
            resetLocaleButton.style.flexGrow = 1;
            localeButtonsRow.Add(resetLocaleButton);
            
            localeSection.Add(localeButtonsRow);
            
            content.Add(localeSection);

            // Theme Section
            var themeSection = CreateSection("Theme");
            
            var themeRow = new VisualElement();
            themeRow.style.flexDirection = FlexDirection.Row;
            themeRow.style.alignItems = Align.Center;
            themeRow.style.marginBottom = 6;
            
            var themeLabel = new Label("Theme:");
            themeLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            themeLabel.style.fontSize = 9;
            themeLabel.style.width = 70;
            themeRow.Add(themeLabel);
            
            _themeDropdown = new DropdownField();
            _themeDropdown.choices = new System.Collections.Generic.List<string>
            {
                "Auto", "Light", "Dark"
            };
            _themeDropdown.value = "Auto";
            _themeDropdown.style.flexGrow = 1;
            _themeDropdown.style.fontSize = 9;
            themeRow.Add(_themeDropdown);
            
            themeSection.Add(themeRow);
            
            var setThemeButton = CreateButton("Set Theme", SetTheme);
            themeSection.Add(setThemeButton);
            
            content.Add(themeSection);

            // Actions Section
            var actionsSection = CreateSection("Actions");
            
            var closeButton = CreateButton("Close No-Codes Screen", CloseScreen, false);
            actionsSection.Add(closeButton);
            
            content.Add(actionsSection);

            // Events Section
            var eventsSection = CreateSection("Events Log");
            
            var clearEventsButton = CreateButton("Clear Events", ClearEvents, false);
            clearEventsButton.style.marginBottom = 6;
            eventsSection.Add(clearEventsButton);
            
            _eventsContainer = new VisualElement();
            _eventsContainer.style.backgroundColor = new Color(0.08f, 0.08f, 0.1f, 1f);
            _eventsContainer.style.borderTopLeftRadius = 4;
            _eventsContainer.style.borderTopRightRadius = 4;
            _eventsContainer.style.borderBottomLeftRadius = 4;
            _eventsContainer.style.borderBottomRightRadius = 4;
            _eventsContainer.style.paddingTop = 6;
            _eventsContainer.style.paddingBottom = 6;
            _eventsContainer.style.paddingLeft = 6;
            _eventsContainer.style.paddingRight = 6;
            _eventsContainer.style.maxHeight = 150;
            _eventsContainer.style.overflow = Overflow.Hidden;
            eventsSection.Add(_eventsContainer);
            
            content.Add(eventsSection);

            ((ScrollView)RootElement).Add(content);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            UpdateStatus();
            UpdateEvents();
        }

        private void UpdateStatus()
        {
            if (_noCodesStatusLabel == null) return;

            if (AppState.NoCodesInitialized)
            {
                _noCodesStatusLabel.text = "✓ Initialized";
                _noCodesStatusLabel.style.color = new Color(0.2f, 0.8f, 0.3f, 1f);
            }
            else
            {
                _noCodesStatusLabel.text = "✗ Not initialized";
                _noCodesStatusLabel.style.color = new Color(0.9f, 0.5f, 0.2f, 1f);
            }

            if (_purchaseDelegateStatusLabel == null) return;

            if (AppState.NoCodesPurchaseDelegateEnabled)
            {
                _purchaseDelegateStatusLabel.text = "✓ Custom Handling Enabled";
                _purchaseDelegateStatusLabel.style.color = new Color(0.2f, 0.8f, 0.3f, 1f);
                
                // Disable the button when already enabled
                if (_enablePurchaseDelegateButton != null)
                {
                    _enablePurchaseDelegateButton.SetEnabled(false);
                    _enablePurchaseDelegateButton.text = "Custom Delegate Active";
                    _enablePurchaseDelegateButton.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
            }
            else
            {
                _purchaseDelegateStatusLabel.text = "Default SDK Flow";
                _purchaseDelegateStatusLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                
                // Enable the button
                if (_enablePurchaseDelegateButton != null)
                {
                    _enablePurchaseDelegateButton.SetEnabled(true);
                    _enablePurchaseDelegateButton.text = "Enable Custom Purchase Delegate";
                    _enablePurchaseDelegateButton.style.backgroundColor = new Color(0.2f, 0.6f, 0.3f, 1f);
                }
            }
        }

        private void UpdateEvents()
        {
            if (_eventsContainer == null) return;
            _eventsContainer.Clear();

            var events = AppState.NoCodesEvents;
            if (events.Count == 0)
            {
                var emptyLabel = new Label("No events yet. Show a screen to see events.");
                emptyLabel.style.color = new Color(0.55f, 0.55f, 0.6f, 1f);
                emptyLabel.style.fontSize = 8;
                emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
                emptyLabel.style.whiteSpace = WhiteSpace.Normal;
                _eventsContainer.Add(emptyLabel);
                return;
            }

            // Show last 15 events (most recent at top)
            int startIndex = Math.Max(0, events.Count - 15);
            for (int i = events.Count - 1; i >= startIndex; i--)
            {
                var eventLabel = new Label(events[i]);
                eventLabel.style.fontSize = 7;
                eventLabel.style.marginBottom = 2;
                eventLabel.style.whiteSpace = WhiteSpace.Normal;
                
                // Color code events
                if (events[i].Contains("✅"))
                    eventLabel.style.color = new Color(0.2f, 0.8f, 0.3f, 1f);
                else if (events[i].Contains("❌"))
                    eventLabel.style.color = new Color(0.9f, 0.3f, 0.3f, 1f);
                else if (events[i].Contains("🛒") || events[i].Contains("🔄"))
                    eventLabel.style.color = new Color(0.3f, 0.6f, 1f, 1f);
                else if (events[i].Contains("⚠️"))
                    eventLabel.style.color = new Color(1f, 0.7f, 0.2f, 1f);
                else
                    eventLabel.style.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                
                _eventsContainer.Add(eventLabel);
            }
        }

        private VisualElement CreateLabeledTextField(string labelText, string defaultValue, out TextField textField)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.overflow = Overflow.Hidden;

            var label = new Label(labelText);
            label.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            label.style.width = 70;
            label.style.fontSize = 9;
            label.style.flexShrink = 0;
            row.Add(label);

            textField = new TextField();
            textField.value = defaultValue;
            textField.style.flexGrow = 1;
            textField.style.fontSize = 9;
            textField.style.minHeight = 24;
            textField.style.backgroundColor = new Color(0.15f, 0.15f, 0.18f, 1f);
            textField.style.borderTopLeftRadius = 4;
            textField.style.borderTopRightRadius = 4;
            textField.style.borderBottomLeftRadius = 4;
            textField.style.borderBottomRightRadius = 4;
            textField.style.paddingLeft = 6;
            textField.style.paddingRight = 6;
            row.Add(textField);

            return row;
        }

        private VisualElement CreatePlatformNote(string text)
        {
            var note = new Label(text);
            note.style.color = new Color(0.55f, 0.55f, 0.6f, 1f);
            note.style.fontSize = 7;
            note.style.unityFontStyleAndWeight = FontStyle.Italic;
            note.style.marginTop = 4;
            note.style.whiteSpace = WhiteSpace.Normal;
            return note;
        }

        private void ShowScreen()
        {
            if (!AppState.NoCodesInitialized)
            {
                AppState.ShowError("No-Codes is not initialized");
                return;
            }

            var contextKey = _contextKeyField?.value?.Trim();
            if (string.IsNullOrEmpty(contextKey))
            {
                AppState.ShowError("Please enter a context key");
                return;
            }

            try
            {
                Debug.Log($"🔄 [NoCodes] Showing screen: {contextKey}");
                NoCodes.GetSharedInstance().ShowScreen(contextKey);
                Debug.Log("✅ [NoCodes] showScreen called");
                AppState.AddNoCodesEvent($"📱 ShowScreen: {contextKey}");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to show screen: {e.Message}");
                AppState.ShowError($"Failed to show screen: {e.Message}");
            }
        }

        private void SetPresentationConfig()
        {
            if (!AppState.NoCodesInitialized)
            {
                AppState.ShowError("No-Codes is not initialized");
                return;
            }

            try
            {
                var styleString = _presentationStyleDropdown?.value ?? "FullScreen";
                var animated = _animatedToggle?.value ?? true;
                var contextKey = _contextKeyField?.value?.Trim();

                ScreenPresentationStyle style = styleString switch
                {
                    "Push" => ScreenPresentationStyle.Push,
                    "Popover" => ScreenPresentationStyle.Popover,
                    "NoAnimation" => ScreenPresentationStyle.NoAnimation,
                    _ => ScreenPresentationStyle.FullScreen
                };

                var config = new ScreenPresentationConfig(style, animated);
                
                Debug.Log($"🔄 [NoCodes] Setting presentation config: {styleString}, animated: {animated}");
                NoCodes.GetSharedInstance().SetScreenPresentationConfig(
                    config, 
                    string.IsNullOrEmpty(contextKey) ? null : contextKey
                );
                
                Debug.Log("✅ [NoCodes] Presentation config set");
                AppState.ShowSuccess("Presentation config set!");
                AppState.AddNoCodesEvent($"⚙️ Config set: {styleString}, animated: {animated}");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to set config: {e.Message}");
                AppState.ShowError($"Failed to set config: {e.Message}");
            }
        }

        private void SetLocale()
        {
            if (!AppState.NoCodesInitialized)
            {
                AppState.ShowError("No-Codes is not initialized");
                return;
            }

            var locale = _localeField?.value?.Trim();
            if (string.IsNullOrEmpty(locale))
            {
                AppState.ShowError("Please enter a locale");
                return;
            }

            try
            {
                Debug.Log($"🔄 [NoCodes] Setting locale: {locale}");
                NoCodes.GetSharedInstance().SetLocale(locale);
                Debug.Log("✅ [NoCodes] Locale set");
                AppState.ShowSuccess($"Locale set to: {locale}");
                AppState.AddNoCodesEvent($"🌍 Locale set: {locale}");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to set locale: {e.Message}");
                AppState.ShowError($"Failed to set locale: {e.Message}");
            }
        }

        private void ResetLocale()
        {
            if (!AppState.NoCodesInitialized)
            {
                AppState.ShowError("No-Codes is not initialized");
                return;
            }

            try
            {
                Debug.Log("🔄 [NoCodes] Resetting locale to device default...");
                NoCodes.GetSharedInstance().SetLocale(null);
                if (_localeField != null) _localeField.value = "";
                Debug.Log("✅ [NoCodes] Locale reset");
                AppState.ShowSuccess("Locale reset to device default");
                AppState.AddNoCodesEvent("🌍 Locale reset to default");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to reset locale: {e.Message}");
                AppState.ShowError($"Failed to reset locale: {e.Message}");
            }
        }

        private void SetTheme()
        {
            if (!AppState.NoCodesInitialized)
            {
                AppState.ShowError("No-Codes is not initialized");
                return;
            }

            try
            {
                var themeString = _themeDropdown?.value ?? "Auto";
                NoCodesTheme theme = themeString switch
                {
                    "Light" => NoCodesTheme.Light,
                    "Dark" => NoCodesTheme.Dark,
                    _ => NoCodesTheme.Auto
                };

                Debug.Log($"🔄 [NoCodes] Setting theme: {themeString}");
                NoCodes.GetSharedInstance().SetTheme(theme);
                Debug.Log("✅ [NoCodes] Theme set");
                AppState.ShowSuccess($"Theme set to: {themeString}");
                AppState.AddNoCodesEvent($"🎨 Theme set: {themeString}");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to set theme: {e.Message}");
                AppState.ShowError($"Failed to set theme: {e.Message}");
            }
        }

        private void CloseScreen()
        {
            if (!AppState.NoCodesInitialized)
            {
                AppState.ShowError("No-Codes is not initialized");
                return;
            }

            try
            {
                Debug.Log("🔄 [NoCodes] Closing...");
                NoCodes.GetSharedInstance().Close();
                Debug.Log("✅ [NoCodes] Closed");
                AppState.ShowSuccess("No-Codes screen closed");
                AppState.AddNoCodesEvent("🚪 Screen closed");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ [NoCodes] Failed to close: {e.Message}");
                AppState.ShowError($"Failed to close: {e.Message}");
            }
        }

        private void ClearEvents()
        {
            AppState.ClearNoCodesEvents();
            AppState.ShowSuccess("Events cleared");
        }

        private void EnablePurchaseDelegate()
        {
            // Find SampleApp and call EnableCustomPurchaseDelegate
            var sampleApp = UnityEngine.Object.FindFirstObjectByType<SampleApp>();
            if (sampleApp != null)
            {
                sampleApp.EnableCustomPurchaseDelegate();
            }
            else
            {
                AppState.ShowError("SampleApp not found");
            }
        }
    }
}
