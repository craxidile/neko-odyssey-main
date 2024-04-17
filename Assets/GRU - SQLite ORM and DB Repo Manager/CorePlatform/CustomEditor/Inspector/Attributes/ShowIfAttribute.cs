#if UNITY_EDITOR
using System;
using UnityEngine;

namespace SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Inspector.Attributes
{
    /// <summary>
    /// example usage: put <i>[ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(showHideList))] </i>
    /// above serializable field that you want to show in inspector this way
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public ActionOnConditionFail Action { get; private set; }
        public ConditionOperator Operator { get; private set; }
        public string[] Conditions { get; private set; }

        public ShowIfAttribute(ActionOnConditionFail action, ConditionOperator conditionOperator, params string[] conditions)
        {
            Action = action;
            Operator = conditionOperator;
            Conditions = conditions;
        }
    }
}
#endif