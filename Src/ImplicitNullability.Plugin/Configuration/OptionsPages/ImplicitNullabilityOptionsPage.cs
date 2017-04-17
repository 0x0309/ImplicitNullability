﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Commands;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.UI;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions.ViewModel;
using JetBrains.UI.Resources;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace ImplicitNullability.Plugin.Configuration.OptionsPages
{
    [ExcludeFromCodeCoverage /* options page user interface is tested manually */]
    [OptionsPage(PageId, PageTitle, typeof(CommonThemedIcons.Bulb), ParentId = CodeInspectionPage.PID)]
    public class ImplicitNullabilityOptionsPage : SimpleOptionsPage
    {
        public const string PageTitle = "Implicit Nullability";
        private const string PageId = "ImplicitNullabilityOptions";

        private static readonly TextStyle Italic = new TextStyle(FontStyle.Italic);

        private readonly ISettingsOptimization _settingsOptimization;
        private readonly Clipboard _clipboard;

        public ImplicitNullabilityOptionsPage(
            Lifetime lifetime,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            ISettingsOptimization settingsOptimization,
            Clipboard clipboard)
            : base(lifetime, optionsSettingsSmartContext)
        {
            _settingsOptimization = settingsOptimization;
            _clipboard = clipboard;

            var enabledOption = AddBoolOption((ImplicitNullabilitySettings s) => s.Enabled, "Enabled");

            // Note: Text duplicated in README
            var infoText =
                "With enabled Implicit Nullability, reference types are by default implicitly [NotNull] for " +
                "specific, configurable elements (see below). " +
                "Their nullability can be overridden with an explicit [CanBeNull] attribute. " +
                "Optional method parameters with a null default value are implicitly [CanBeNull].\n" +
                "\n" +
                "Code elements which should be affected by Implicit Nullability can be configured in two ways.\n" +
                "\n" +
                "1. Recommended for application authors: Using the following settings.";
            SetIndent(AddText(infoText), 1);


            AddNullabilityBoolOption(
                s => s.EnableInputParameters,
                "Input parameters of methods, delegates, and indexers",
                enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.EnableRefParameters,
                "Ref parameters of methods and delegates",
                enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.EnableOutParametersAndResult,
                "Return values and out parameters of methods and delegates",
                enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.EnableFields,
                "Fields",
                enabledOption,
                indent: 2);

            AddNullabilityBoolOption(
                s => s.FieldsRestrictToReadonly,
                new RichText("Restrict to ") +
                new RichText("readonly", Italic) +
                new RichText(" fields (because ") +
                new RichText("readonly", Italic) +
                new RichText(" fields can be guarded by invariants / constructor checks)"),
                enabledOption,
                indent: 3);

            AddNullabilityBoolOption(
                s => s.FieldsRestrictToReferenceTypes,
                new RichText("Restrict to fields contained in reference types (because ") +
                new RichText("struct", Italic) +
                new RichText("s cannot be guarded by invariants due to their implicit default constructor)"),
                enabledOption,
                indent: 3);

            var assemblyAttributeInfoText1 =
                "\n" +
                "2. Recommended for library authors: By defining an [AssemblyMetadata] attribute in all concerned assemblies. "
                +
                "This has the advantage of the Implicit Nullability configuration also getting compiled into the assemblies.";
            SetIndent(AddText(assemblyAttributeInfoText1), 1);

            var copyButtonText = "Copy [AssemblyMetadata] attribute to clipboard (using above options as a template)";
            var copyButton = AddButton(copyButtonText, new DelegateCommand(CopyAssemblyAttributeCode));
            SetIndent(copyButton, 2);
            enabledOption.CheckedProperty.FlowInto(myLifetime, copyButton.GetIsEnabledProperty());

            var assemblyAttributeInfoText2 =
                "Implicit Nullability normally ignores referenced assemblies. " +
                "It can take referenced libraries into account only if they contain embedded [AssemblyMetadata]-based configuration.\n" +
                "\n" +
                "The options in a [AssemblyMetadata] attribute override the options selected above.";
            SetIndent(AddText(assemblyAttributeInfoText2), 1);

            var cacheInfoText =
                "Warning: After changing these settings respectively changing the [AssemblyMetadata] attribute value, " +
                "cleaning the solution cache (see \"General\" options page) " +
                "is necessary to update already analyzed code.";
            AddText(cacheInfoText);

            AddText("\n");
            AddBoolOption(
                (ImplicitNullabilitySettings s) => s.EnableTypeHighlighting,
                "Enable type highlighting of explicit or implicit [NotNull] elements " +
                "(to adapt the color, look for \"Implicit Nullability\" in Visual Studio's \"Fonts and Colors\" options)");
        }

        private void AddNullabilityBoolOption(
            Expression<Func<ImplicitNullabilitySettings, bool>> settingsExpression,
            string text,
            BoolOptionViewModel enabledOption,
            int indent)
        {
            AddNullabilityBoolOption(settingsExpression, new RichText(text), enabledOption, indent);
        }

        private void AddNullabilityBoolOption(
            Expression<Func<ImplicitNullabilitySettings, bool>> settingsExpression,
            RichText richText,
            BoolOptionViewModel enabledOption,
            int indent)
        {
            var boolOption = AddBoolOption(settingsExpression, richText);
            enabledOption.CheckedProperty.FlowInto(myLifetime, boolOption.GetIsEnabledProperty());
            SetIndent(boolOption, indent);
        }

        private void CopyAssemblyAttributeCode()
        {
            var currentImplicitNullabilitySettings =
                OptionsSettingsSmartContext.GetKey<ImplicitNullabilitySettings>(_settingsOptimization);

            var assemblyMetadataCode = AssemblyAttributeConfigurationTranslator.GenerateAttributeCode(
                ImplicitNullabilityConfiguration.CreateFromSettings(currentImplicitNullabilitySettings));

            _clipboard.SetText(assemblyMetadataCode);
            MessageBox.ShowInfo("The following code has been copied to your clipboard:\n\n\n" + assemblyMetadataCode);
        }
    }
}