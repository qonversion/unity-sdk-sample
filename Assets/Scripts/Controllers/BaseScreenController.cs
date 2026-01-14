using UnityEngine.UIElements;

namespace QonversionSample
{
    /// <summary>
    /// Base class for all screen controllers.
    /// Provides common functionality for screen management.
    /// </summary>
    public abstract class BaseScreenController
    {
        protected readonly AppState AppState;
        protected VisualElement RootElement;

        protected BaseScreenController(AppState appState)
        {
            AppState = appState;
        }

        /// <summary>
        /// Creates and returns the UI for this screen.
        /// </summary>
        public abstract VisualElement CreateUI();

        /// <summary>
        /// Called when the screen is shown.
        /// </summary>
        public virtual void OnShow() { }

        /// <summary>
        /// Called when the screen is hidden.
        /// </summary>
        public virtual void OnHide() { }

        /// <summary>
        /// Called to refresh the UI when state changes.
        /// </summary>
        public virtual void Refresh() { }

        /// <summary>
        /// Creates a styled section container with a title.
        /// </summary>
        protected VisualElement CreateSection(string title)
        {
            var section = new VisualElement();
            section.style.marginBottom = 6;
            section.style.backgroundColor = new UnityEngine.Color(0.11f, 0.11f, 0.13f, 1f);
            section.style.borderTopLeftRadius = 6;
            section.style.borderTopRightRadius = 6;
            section.style.borderBottomLeftRadius = 6;
            section.style.borderBottomRightRadius = 6;
            section.style.paddingTop = 6;
            section.style.paddingBottom = 6;
            section.style.paddingLeft = 8;
            section.style.paddingRight = 8;
            section.style.overflow = Overflow.Hidden;
            section.style.flexShrink = 1;

            var titleLabel = new Label(title.ToUpper());
            titleLabel.style.fontSize = 8;
            titleLabel.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            titleLabel.style.marginBottom = 4;
            titleLabel.style.color = new UnityEngine.Color(0.45f, 0.45f, 0.5f, 1f);
            titleLabel.style.letterSpacing = 1;
            section.Add(titleLabel);

            return section;
        }

        /// <summary>
        /// Creates a styled button.
        /// </summary>
        protected Button CreateButton(string text, System.Action onClick, bool isPrimary = true)
        {
            var button = new Button(onClick) { text = text };
            button.style.paddingTop = 5;
            button.style.paddingBottom = 5;
            button.style.paddingLeft = 10;
            button.style.paddingRight = 10;
            button.style.marginTop = 1;
            button.style.marginBottom = 1;
            button.style.borderTopLeftRadius = 4;
            button.style.borderTopRightRadius = 4;
            button.style.borderBottomLeftRadius = 4;
            button.style.borderBottomRightRadius = 4;
            button.style.borderTopWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.fontSize = 9;
            button.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            button.style.flexShrink = 1;
            button.style.whiteSpace = WhiteSpace.Normal;

            if (isPrimary)
            {
                button.style.backgroundColor = new UnityEngine.Color(0.2f, 0.5f, 1f, 1f);
                button.style.color = UnityEngine.Color.white;
            }
            else
            {
                button.style.backgroundColor = new UnityEngine.Color(0.18f, 0.18f, 0.2f, 1f);
                button.style.color = new UnityEngine.Color(0.85f, 0.85f, 0.85f, 1f);
            }

            return button;
        }

        /// <summary>
        /// Creates an info row with label and value.
        /// </summary>
        protected VisualElement CreateInfoRow(string label, string value)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.alignItems = Align.FlexStart;
            row.style.paddingTop = 3;
            row.style.paddingBottom = 3;
            row.style.borderBottomWidth = 1;
            row.style.borderBottomColor = new UnityEngine.Color(0.15f, 0.15f, 0.17f, 1f);
            row.style.overflow = Overflow.Hidden;

            var labelElement = new Label(label);
            labelElement.style.color = new UnityEngine.Color(0.55f, 0.55f, 0.6f, 1f);
            labelElement.style.fontSize = 8;
            labelElement.style.flexShrink = 0;
            labelElement.style.minWidth = 50;
            row.Add(labelElement);

            var valueElement = new Label(value ?? "—");
            valueElement.style.color = new UnityEngine.Color(0.9f, 0.9f, 0.9f, 1f);
            valueElement.style.fontSize = 8;
            valueElement.style.unityTextAlign = UnityEngine.TextAnchor.MiddleRight;
            valueElement.style.flexShrink = 1;
            valueElement.style.flexGrow = 1;
            valueElement.style.whiteSpace = WhiteSpace.Normal;
            valueElement.style.overflow = Overflow.Hidden;
            row.Add(valueElement);

            return row;
        }

        /// <summary>
        /// Creates a scrollable container.
        /// </summary>
        protected ScrollView CreateScrollView()
        {
            var scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            return scrollView;
        }

        /// <summary>
        /// Creates a card container for items.
        /// </summary>
        protected VisualElement CreateCard()
        {
            var card = new VisualElement();
            card.style.backgroundColor = new UnityEngine.Color(0.11f, 0.11f, 0.13f, 1f);
            card.style.borderTopLeftRadius = 6;
            card.style.borderTopRightRadius = 6;
            card.style.borderBottomLeftRadius = 6;
            card.style.borderBottomRightRadius = 6;
            card.style.paddingTop = 6;
            card.style.paddingBottom = 6;
            card.style.paddingLeft = 8;
            card.style.paddingRight = 8;
            card.style.marginBottom = 4;
            card.style.overflow = Overflow.Hidden;
            card.style.flexShrink = 1;
            return card;
        }

        /// <summary>
        /// Creates a chip/badge element.
        /// </summary>
        protected VisualElement CreateChip(string text, UnityEngine.Color backgroundColor)
        {
            var chip = new Label(text);
            chip.style.backgroundColor = backgroundColor;
            chip.style.color = UnityEngine.Color.white;
            chip.style.paddingTop = 2;
            chip.style.paddingBottom = 2;
            chip.style.paddingLeft = 5;
            chip.style.paddingRight = 5;
            chip.style.borderTopLeftRadius = 3;
            chip.style.borderTopRightRadius = 3;
            chip.style.borderBottomLeftRadius = 3;
            chip.style.borderBottomRightRadius = 3;
            chip.style.fontSize = 7;
            chip.style.marginRight = 3;
            chip.style.marginBottom = 2;
            return chip;
        }
    }
}
