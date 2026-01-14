using UnityEngine;
using UnityEngine.UIElements;
using QonversionUnity;

namespace QonversionSample
{
    /// <summary>
    /// Controller for the User screen.
    /// Handles user identification, properties, and info.
    /// </summary>
    public class UserScreenController : BaseScreenController
    {
        private VisualElement _userInfoContainer;
        private VisualElement _propertiesContainer;
        private TextField _identifyField;
        private TextField _propertyKeyField;
        private TextField _propertyValueField;
        private Label _propertiesEmptyLabel;

        public UserScreenController(AppState appState) : base(appState) { }

        public override VisualElement CreateUI()
        {
            RootElement = CreateScrollView();

            var content = new VisualElement();
            content.style.paddingTop = 16;

            // Header
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 6;

            var title = new Label("User");
            title.style.fontSize = 11;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            header.Add(title);

            var reloadButton = CreateButton("Reload", ReloadUserInfo);
            header.Add(reloadButton);

            content.Add(header);

            // User Info Section
            var userInfoSection = CreateSection("User Info");
            _userInfoContainer = new VisualElement();
            userInfoSection.Add(_userInfoContainer);
            content.Add(userInfoSection);

            // Identify Section
            var identifySection = CreateSection("Identify User");
            
            var identifyRow = new VisualElement();
            identifyRow.style.flexDirection = FlexDirection.Row;
            identifyRow.style.alignItems = Align.Center;
            identifyRow.style.marginBottom = 6;

            _identifyField = new TextField();
            _identifyField.style.flexGrow = 1;
            _identifyField.style.marginRight = 8;
            StyleTextField(_identifyField, "User ID");
            identifyRow.Add(_identifyField);

            var identifyButton = CreateButton("Identify", IdentifyUser);
            identifyButton.style.height = 28;
            identifyButton.style.paddingLeft = 14;
            identifyButton.style.paddingRight = 14;
            identifyRow.Add(identifyButton);

            identifySection.Add(identifyRow);

            var logoutButton = CreateButton("Logout", Logout, false);
            logoutButton.style.height = 28;
            identifySection.Add(logoutButton);

            content.Add(identifySection);

            // Set User Property Section
            var propertySection = CreateSection("Set Custom User Property");
            
            var propertyKeyRow = new VisualElement();
            propertyKeyRow.style.flexDirection = FlexDirection.Row;
            propertyKeyRow.style.marginBottom = 6;

            _propertyKeyField = new TextField();
            _propertyKeyField.style.flexGrow = 1;
            StyleTextField(_propertyKeyField, "Key");
            propertyKeyRow.Add(_propertyKeyField);
            propertySection.Add(propertyKeyRow);

            var propertyValueRow = new VisualElement();
            propertyValueRow.style.flexDirection = FlexDirection.Row;
            propertyValueRow.style.marginBottom = 8;

            _propertyValueField = new TextField();
            _propertyValueField.style.flexGrow = 1;
            StyleTextField(_propertyValueField, "Value");
            propertyValueRow.Add(_propertyValueField);
            propertySection.Add(propertyValueRow);

            var setPropertyButton = CreateButton("Set Property", SetCustomProperty);
            setPropertyButton.style.height = 28;
            propertySection.Add(setPropertyButton);

            content.Add(propertySection);

            // User Properties Section
            var propertiesSection = CreateSection("User Properties");
            
            var loadPropsButton = CreateButton("Load Properties", LoadUserProperties);
            loadPropsButton.style.height = 28;
            loadPropsButton.style.marginBottom = 8;
            propertiesSection.Add(loadPropsButton);

            _propertiesEmptyLabel = new Label("Click 'Load Properties' to view.");
            _propertiesEmptyLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            _propertiesEmptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            _propertiesEmptyLabel.style.fontSize = 9;
            propertiesSection.Add(_propertiesEmptyLabel);

            _propertiesContainer = new VisualElement();
            propertiesSection.Add(_propertiesContainer);

            content.Add(propertiesSection);

            ((ScrollView)RootElement).Add(content);

            Refresh();
            return RootElement;
        }

        public override void Refresh()
        {
            UpdateUserInfo();
            UpdateProperties();
        }

        private void UpdateUserInfo()
        {
            if (_userInfoContainer == null) return;
            _userInfoContainer.Clear();

            var user = AppState.UserInfo;
            if (user == null)
            {
                var noUserLabel = new Label("No user info loaded. Click 'Reload' to fetch.");
                noUserLabel.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                noUserLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
                _userInfoContainer.Add(noUserLabel);
                return;
            }

            _userInfoContainer.Add(CreateInfoRow("Qonversion ID", user.QonversionId));
            _userInfoContainer.Add(CreateInfoRow("Identity ID", user.IdentityId ?? "Not identified"));
        }

        private void UpdateProperties()
        {
            if (_propertiesContainer == null) return;
            _propertiesContainer.Clear();

            var properties = AppState.UserProperties;
            if (properties?.Properties == null || properties.Properties.Count == 0)
            {
                _propertiesEmptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            _propertiesEmptyLabel.style.display = DisplayStyle.None;

            // Defined properties
            if (properties.DefinedProperties != null && properties.DefinedProperties.Count > 0)
            {
                var definedHeader = new Label("Defined Properties");
                definedHeader.style.fontSize = 8;
                definedHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                definedHeader.style.color = new Color(0.2f, 0.7f, 0.9f, 1f);
                definedHeader.style.marginTop = 12;
                definedHeader.style.marginBottom = 8;
                _propertiesContainer.Add(definedHeader);

                foreach (var prop in properties.DefinedProperties)
                {
                    _propertiesContainer.Add(CreateInfoRow(prop.Key, prop.Value));
                }
            }

            // Custom properties
            if (properties.CustomProperties != null && properties.CustomProperties.Count > 0)
            {
                var customHeader = new Label("Custom Properties");
                customHeader.style.fontSize = 8;
                customHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                customHeader.style.color = new Color(0.9f, 0.7f, 0.2f, 1f);
                customHeader.style.marginTop = 12;
                customHeader.style.marginBottom = 8;
                _propertiesContainer.Add(customHeader);

                foreach (var prop in properties.CustomProperties)
                {
                    _propertiesContainer.Add(CreateInfoRow(prop.Key, prop.Value));
                }
            }
        }

        private void ReloadUserInfo()
        {
            Debug.Log("🔄 [Qonversion] Reloading user info...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().UserInfo((user, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load user info: {error.Message}");
                    AppState.ShowError($"Failed to load user info: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] User info loaded: {user.QonversionId}");
                AppState.SetUserInfo(user);
                AppState.ShowSuccess("User info reloaded");
            });
        }

        private void IdentifyUser()
        {
            var userId = _identifyField?.value;

            if (string.IsNullOrEmpty(userId))
            {
                AppState.ShowError("Please enter a user ID");
                return;
            }

            Debug.Log($"🔄 [Qonversion] Identifying user: {userId}...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().Identify(userId, (user, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to identify user: {error.Message}");
                    AppState.ShowError($"Failed to identify: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] User identified: {user.QonversionId}");
                AppState.SetUserInfo(user);
                AppState.ShowSuccess($"User identified: {userId}");
            });
        }

        private void Logout()
        {
            Debug.Log("🔄 [Qonversion] Logging out...");

            try
            {
                Qonversion.GetSharedInstance().Logout();
                Debug.Log("✅ [Qonversion] User logged out");
                AppState.ShowSuccess("User logged out");
                
                // Reload user info
                ReloadUserInfo();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Logout failed: {e.Message}");
                AppState.ShowError($"Logout failed: {e.Message}");
            }
        }

        private void SetCustomProperty()
        {
            var key = _propertyKeyField?.value;
            var value = _propertyValueField?.value;

            if (string.IsNullOrEmpty(key))
            {
                AppState.ShowError("Please enter a property key");
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                AppState.ShowError("Please enter a property value");
                return;
            }

            Debug.Log($"🔄 [Qonversion] Setting custom property: {key} = {value}...");

            try
            {
                Qonversion.GetSharedInstance().SetCustomUserProperty(key, value);
                Debug.Log($"✅ [Qonversion] Custom property set: {key}");
                AppState.ShowSuccess($"Property set: {key} = {value}");

                // Clear fields
                _propertyKeyField.value = "";
                _propertyValueField.value = "";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [Qonversion] Failed to set property: {e.Message}");
                AppState.ShowError($"Failed to set property: {e.Message}");
            }
        }

        private void LoadUserProperties()
        {
            Debug.Log("🔄 [Qonversion] Loading user properties...");
            AppState.SetLoading(true);

            Qonversion.GetSharedInstance().UserProperties((properties, error) =>
            {
                AppState.SetLoading(false);

                if (error != null)
                {
                    Debug.LogError($"❌ [Qonversion] Failed to load properties: {error.Message}");
                    AppState.ShowError($"Failed to load properties: {error.Message}");
                    return;
                }

                Debug.Log($"✅ [Qonversion] User properties loaded: {properties?.Properties?.Count ?? 0}");
                AppState.SetUserProperties(properties);
                AppState.ShowSuccess($"Loaded {properties?.Properties?.Count ?? 0} properties");
            });
        }

        private void StyleTextField(TextField field, string placeholder)
        {
            field.style.fontSize = 11;
            field.style.minHeight = 32;
            field.style.backgroundColor = new Color(0.15f, 0.15f, 0.18f, 1f);
            field.style.borderTopLeftRadius = 4;
            field.style.borderTopRightRadius = 4;
            field.style.borderBottomLeftRadius = 4;
            field.style.borderBottomRightRadius = 4;
            field.style.borderTopWidth = 1;
            field.style.borderBottomWidth = 1;
            field.style.borderLeftWidth = 1;
            field.style.borderRightWidth = 1;
            field.style.borderTopColor = new Color(0.25f, 0.25f, 0.28f, 1f);
            field.style.borderBottomColor = new Color(0.25f, 0.25f, 0.28f, 1f);
            field.style.borderLeftColor = new Color(0.25f, 0.25f, 0.28f, 1f);
            field.style.borderRightColor = new Color(0.25f, 0.25f, 0.28f, 1f);
            field.style.paddingLeft = 8;
            field.style.paddingRight = 8;
            field.style.paddingTop = 6;
            field.style.paddingBottom = 6;
            
            // Set placeholder as label (Unity TextField doesn't support native placeholder)
            field.label = "";
            field.value = "";
            
            // Add placeholder overlay
            var placeholderLabel = new Label(placeholder);
            placeholderLabel.style.position = Position.Absolute;
            placeholderLabel.style.left = 8;
            placeholderLabel.style.top = 0;
            placeholderLabel.style.bottom = 0;
            placeholderLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            placeholderLabel.style.color = new Color(0.45f, 0.45f, 0.5f, 1f);
            placeholderLabel.style.fontSize = 10;
            placeholderLabel.pickingMode = PickingMode.Ignore;
            field.Add(placeholderLabel);
            
            field.RegisterValueChangedCallback(evt => 
            {
                placeholderLabel.style.display = string.IsNullOrEmpty(evt.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
            });
        }
    }
}
