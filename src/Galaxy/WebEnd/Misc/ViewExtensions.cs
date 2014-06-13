﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Codestellation.Galaxy.WebEnd.TagBuilder;
using Nancy.ViewEngines.Razor;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public static class ViewExtensions
    {
        public static IHtmlString LabeledTextBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            Tag input = Tags.Input.Text();

            return BuildInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledNumberBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            Tag input = Tags.Input.Number();

            return BuildInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledPasswordBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, TModel instance = default(TModel))
        {
            return BuildInput(htmlHelper, property, Tags.Input.Password());
        }

        public static IHtmlString LabeledCheckBox<TModel>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, bool>> property)
        {
            var member = property.GetMember();
            var path = property.ToMemberPath();
            var display = member.GetDisplay();

            bool value = Reader.Read(htmlHelper.Model, property);

            var formGroup = Tags.Div().Classes(BootstrapClass.FormGroup).Content(
                Tags.Div().Classes(BootstrapClass.ColSmOffset2, BootstrapClass.ColSm10).Content(
                    Tags.Div().Classes(BootstrapClass.Checkbox).Content(
                        Tags.Label().Content(
                            Tags.Input.CheckBox().Checked(value).Name(path).Content(display.Name)
                            )
                        )
                    )
                );
/*            
            const string template = @"
<div class=""form-group"">
    <div class=""col-sm-offset-2 col-sm-10"">
      <div class=""checkbox"">
        <label>
          <input name=""{0}"" type=""checkbox""{1} value=""true""> {2}
        </label>
      </div>
    </div>
</div>";*/

            return formGroup;
        }

        private static IHtmlString BuildInput<TModel, TProperty>(HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, Tag input)
        {
            var member = property.GetMember();
            var path = property.ToMemberPath();
            var display = member.GetDisplay();

            TProperty value = Reader.Read(htmlHelper.Model, property);


            input.Classes(BootstrapClass.FormControl)
                .Id(path)
                .Name(path)
                .Value(value);

            if (!string.IsNullOrWhiteSpace(display.Prompt))
            {
                input.Placeholder(display.GetPrompt());
            }

            return BuildFormControl(path, display, input);
        }

        private static IHtmlString BuildFormControl(string path, DisplayAttribute display, Tag input)
        {
            var formGroup = Tags.Div().Classes(BootstrapClass.FormGroup).Content(
                Tags.Label().For(path).Classes(BootstrapClass.ColSm2, BootstrapClass.ControlLabel).Content(display.Name),
                Tags.Div().Classes(BootstrapClass.ColSm4).Content(input));

            return new NonEncodedHtmlString(formGroup.ToHtmlString());
        }
    }
}