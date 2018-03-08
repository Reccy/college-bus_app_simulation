﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EditorGUITable
{

	/// <summary>
	/// This entry class draws a string as a label.
	/// This is useful for properties you want to display in the table
	/// as read only, as the default PropertyField used in PropertyEntry uses editable fields.
	/// </summary>
	public class LabelEntry : TableEntry
	{

		string value;

		public override void DrawEntryLayout (float width, float height)
		{
			EditorGUILayout.LabelField (value, GUILayout.Width (width), GUILayout.Height (height));
		}

		public override void DrawEntry (Rect rect)
		{
			GUI.Label(rect, value);
		}

		public override string comparingValue
		{
			get
			{
				return value;
			}
		}

		public LabelEntry (string value)
		{
			this.value = value;
		}
	}

}
