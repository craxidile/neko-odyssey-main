#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SpatiumInteractive.Libraries.Unity.Platform.Configs;
using System.Linq;

namespace SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Window
{
    public static class EditorWindowBaseExtension
    {
        public static TwoPaneSplitView AddNewVerticalSplitView(this EditorWindowBase form, float minSizeHeight = default)
        {
            minSizeHeight = minSizeHeight == default ? Config.Editor.Window.PaneSplitView.DEFAULT_MIN_SIZE_HEIGHT : minSizeHeight;

            var splitView = new TwoPaneSplitView
            (
                0,
                minSizeHeight,
                TwoPaneSplitViewOrientation.Vertical
            );
            var topPane = new VisualElement();
            var bottomPane = new VisualElement();

            splitView.Add(topPane);
            splitView.Add(bottomPane);

            form.rootVisualElement.Add(splitView);

            return splitView;
        }

        public static Image GetImageForSprite(this EditorWindowBase form, string imagePath)
        {
            Sprite logoSprite = (Sprite)AssetDatabase.LoadAssetAtPath(imagePath, typeof(Sprite));
            var spriteImage = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                sprite = logoSprite
            };
            return spriteImage;
        }

        public static EditorWindowBase AddNewImageElement(this EditorWindowBase form, Image image, VisualElement parent = null, float height = 100, string tooltip = "", float paddingBottom = 0, float marginLeft = 0)
        {
            if (parent != null)
            {
                parent.Add(image);
            }
            else
            {
                form.rootVisualElement.Add(image);
            }

            image.style.height = height;
            image.tooltip = tooltip;
            image.style.paddingBottom = paddingBottom;
            image.style.marginLeft = marginLeft;

            return form;
        }

        public static EditorWindowBase AddNewInputElement(this EditorWindowBase form, string fieldName, Action onChangeCallback = null, string label = "", VisualElement parent = null, bool isEnabled = true, int marginTop = 0, int height = 30)
        {
            var newField = new PropertyField() { bindingPath = fieldName };
            newField.SetEnabled(isEnabled);

            if (onChangeCallback != null)
            {
                newField.RegisterCallback<ChangeEvent<string>>(x => onChangeCallback());
            }

            if (parent != null)
            {
                parent.Add(newField);
            }

            newField.label = label;
            newField.style.marginTop = marginTop;
            newField.style.height = height;

            return form;
        }

        public static EditorWindowBase AddNewScrollView(this EditorWindowBase form, VisualElement parent, ref ScrollView scrollViewRef, ScrollViewMode scrollViewMode = ScrollViewMode.Vertical)
        {
            var newScrollView = new ScrollView(scrollViewMode);
            if (parent != null)
            {
                parent.Add(newScrollView);
            }
            else
            {
                form.rootVisualElement.Add(newScrollView);
            }

            scrollViewRef = newScrollView;

            return form;
        }

        /// <summary>
        /// adds new vertically scrollable checkbox list
        /// </summary>
        /// <param name="checkboxLabels">checkboxLabels and checkboxUser Data must be of same length</param>
        /// <param name="chekboxUserData">checkboxLabels and checkboxUser Data must be of same length</param>
        /// <returns></returns>
        public static EditorWindowBase AddNewScrollableCheckboxList
        (
            this EditorWindowBase form,
            List<string> checkboxLabels,
            List<object> chekboxUserData,
            Func<Toggle, List<Toggle>> cbxClickedCallback,
            ref List<Toggle> checkboxListRef,
            VisualElement parent = null,
            bool defaultIsChecked = false,
            bool defaultIsEnabled = true
        )
        {
            var scrollableListView = new ScrollView(ScrollViewMode.Vertical);
            scrollableListView.style.height = 100;
            for (int i = 0; i < checkboxLabels.Count; i++)
            {
                var label = checkboxLabels[i];
                var newCheckbox = new Toggle(label);
                newCheckbox.value = defaultIsChecked;
                newCheckbox.SetEnabled(defaultIsEnabled);
                newCheckbox.userData = chekboxUserData?[i]; //todo: could be error prone in case labels list is not equal length of user data list .. kwp is better choice, will be added in future GRU versions
                scrollableListView.Add(newCheckbox);
                newCheckbox.RegisterValueChangedCallback(x => cbxClickedCallback(newCheckbox));
                newCheckbox.style.backgroundColor = Color.gray;
                checkboxListRef.Add(newCheckbox);
            }

            if (parent != null)
            {
                parent.Add(scrollableListView);
            }
            else
            {
                form.rootVisualElement.Add(scrollableListView);
            }

            return form;
        }

        public static EditorWindowBase AddNewScrollableButtonsList
        (
            this EditorWindowBase form,
            List<string> buttonTitles,
            List<object> buttonUserData,
            Action<Button> btnClickedCallback,
            ref List<Button> buttonListRef,
            VisualElement parent = null,
            bool defaultIsEnabled = true,
            int buttonHeight = 30,
            int listHeight = 100
        )
        {
            var scrollableListView = new ScrollView(ScrollViewMode.Vertical);
            scrollableListView.style.height = listHeight;

            for (int i = 0; i < buttonTitles.Count; i++)
            {
                var title = buttonTitles[i];
                var newButton = new Button();
                newButton.text = title;
                newButton.style.height = buttonHeight;
                newButton.SetEnabled(defaultIsEnabled);
                newButton.userData = buttonUserData?[i]; //todo: could be error prone in case text title list is not equal length of user data list .. kwp is better choice?
                scrollableListView.Add(newButton);
                newButton.RegisterCallback<MouseUpEvent>(x => btnClickedCallback(newButton)); //MouseUpEvent doesn't eat up left mouse click ! MouseDownEvent does! This is a know issue - Button class simply eats it - not propagatign it further down the hierarchy tree.. So don't change this. At least not atm.
                newButton.style.backgroundColor = Color.gray;
                buttonListRef.Add(newButton);
            }

            if (parent != null)
            {
                parent.Add(scrollableListView);
            }
            else
            {
                form.rootVisualElement.Add(scrollableListView);
            }

            return form;
        }

        public static EditorWindowBase AddNewCheckbox(this EditorWindowBase form, ref Toggle cbxRef, string label, string tooltip = "", Action onClickCallback = null, bool isCheckedByDefault = false, bool isEnabled = true, VisualElement parent = null)
        {
            Toggle checkBox = new Toggle(label);
            checkBox.SetEnabled(isEnabled);
            checkBox.value = isCheckedByDefault;
            checkBox.tooltip = tooltip;

            if (parent == null)
            {
                form.rootVisualElement.Add(checkBox);
            }
            else
            {
                parent.Add(checkBox);
            }

            cbxRef = checkBox;

            return form;
        }

        public static EditorWindowBase AddNewDropDown(this EditorWindowBase form, ref DropdownField ddlRef, string label, List<string> choices, string tooltip = "", Action<DropdownField> onClickCallback = null, bool isEnabled = true, bool preSelectFirstChoiceAsDefault = true, float marginTop = 10, VisualElement parent = null)
        {
            DropdownField ddl = new DropdownField();
            ddl.choices = choices;
            ddl.label = label;
            ddl.tooltip = tooltip;
            ddl.style.marginTop = marginTop;
            ddl.RegisterValueChangedCallback(x => onClickCallback(ddl));

            if (preSelectFirstChoiceAsDefault && choices.Any())
            {
                ddl.value = ddl.choices[0];
            }

            if (parent == null)
            {
                form.rootVisualElement.Add(ddl);
            }
            else
            {
                parent.Add(ddl);
            }

            ddlRef = ddl;

            return form;
        }

        //todo: either fix this later or delete, seems like this kind of overloading with empty ref for button is not allowed in editor scripting
        //[Obsolete]
        public static EditorWindowBase AddNewButton
        (
            this EditorWindowBase form, 
            string text = Config.Editor.Window.Button.DEFAULT_TITLE, 
            int height = 6, 
            LengthUnit lengthUnit = LengthUnit.Percent, 
            Action onClickCallback = null, 
            bool isEnabled = true, 
            Color? color = null,
            VisualElement parent = null,
            float marginTop = 0
        )
        {
            Button buttonRef = null;
            return AddNewButton(form, ref buttonRef, color.Value, text, height, lengthUnit, onClickCallback, isEnabled, parent, marginTop);
        }

        public static EditorWindowBase AddNewButton
        (
            this EditorWindowBase form,
            ref Button buttonRef,
            Color backgroundColor,
            string text = Config.Editor.Window.Button.DEFAULT_TITLE,
            int height = 6,
            LengthUnit lengthUnit = LengthUnit.Percent,
            Action onClickCallback = null,
            bool isEnabled = true,
            VisualElement parent = null,
            float marginTop = 0
        )
        {
            var button = new Button(onClickCallback)
            {
                text = text
            };
            button.SetEnabled(isEnabled);
            button.style.height = new Length(height, LengthUnit.Percent);
            button.style.backgroundColor = backgroundColor != null ? backgroundColor : Color.gray;
            button.style.marginTop = marginTop;

            if (parent == null)
            {
                form.rootVisualElement.Add(button);
            }
            else
            {
                parent.Add(button);
            }
            buttonRef = button;
            return form;
        }

        public static EditorWindowBase AddNewLabelIfConditionTrue(this EditorWindowBase form, string label, Color color, VisualElement parent = null, int fontSize = 15, int marginTop = 10, bool condition = false)
        {
            if (condition)
            {
                AddNewLabel(form, label, color, parent, fontSize, marginTop);
            }
            return form;
        }

        public static EditorWindowBase AddNewLabel(this EditorWindowBase form, string label, Color color, VisualElement parent = null, int fontSize = 15, int marginTop = 10, int marginLeft = 10)
        {
            var newLabel = new Label(label);
            if (parent != null)
            {
                parent.Add(newLabel);
            }
            else
            {
                form.rootVisualElement.Add(newLabel);
            }

            newLabel.style.marginTop = marginTop;
            newLabel.style.marginLeft = marginLeft;
            newLabel.style.fontSize = fontSize;
            newLabel.style.color = color;

            return form;
        }

        public static void BindAll(this EditorWindowBase form)
        {
            form.rootVisualElement.Bind(new SerializedObject(form));
        }
    }
}

#endif
