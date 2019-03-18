using System;
using Gtk;
using System.Collections.Generic;

namespace GeneToAnno
{
	public abstract class SettingBinder
	{
		public abstract void FillForm();
		public abstract bool ReturnSettings(out string msg);
		public abstract void SensitizeDependents();
	}
	public class SettingBinder<T> : SettingBinder
	{
		protected Widget settingHolder;
		protected List<Widget> dependents;
		protected SettingsItem<T> theItem;

		public bool IsFulfilled { get { if (typeof(T) == typeof(bool)) {
					if (settingHolder.Sensitive) {
						return (settingHolder as CheckButton).Active;
					} else {
						return settingHolder.Sensitive;
					}
				} else {
					return false;			
				}
			}
		}

		public SettingBinder (SettingsItem<T> item, Widget _settingHolder, List<Widget> dependees)
		{
			theItem = item;
			settingHolder = _settingHolder;
			dependents = dependees;
		}

		public SettingBinder (SettingsItem<T> item, Widget _settingHolder)
		{
			theItem = item;
			settingHolder = _settingHolder;

			dependents = null;
		}

		public override void FillForm()
		{
			SendTypeToWidget ();
		}
		public override bool ReturnSettings (out string msg)
		{
			if (settingHolder.Sensitive) {
				bool allInOrder = false;

				allInOrder = ConvertContentToType ();

				if (!allInOrder) {
					msg = "Failed to assign setting from: " + settingHolder.Name;
				} else {
					msg = "";
				}
				

				return allInOrder;
			} else {
				msg = "";
				return true;
			}
		}

		public override void SensitizeDependents()
		{
			if (dependents != null) {
				foreach (Widget wid in dependents) {
					wid.Sensitive = IsFulfilled;
				}
			}
		}

		protected void SendTypeToWidget()
		{
			if (theItem.ThisType.Equals(typeof(string))) {
				Entry ent = settingHolder as Entry;
				ent.Text = (theItem.Item as string);

			} else if (theItem.ThisType.Equals(typeof(bool))) {
				CheckButton chk = settingHolder as CheckButton;
				chk.Active = (bool)(object)theItem.Item;

			} else if (theItem.ThisType.Equals(typeof(List<string>))) {
				Entry ent = settingHolder as Entry;
				int cnt = 0;
				foreach (string st in (theItem.Item as List<string>)) {
					if (cnt > 0) {
						ent.Text += ";";	
					}
					ent.Text += st;
					cnt++;
				}
			} else if (theItem.ThisType.Equals(typeof(int))) {
				Entry ent = settingHolder as Entry;
				ent.Text = ((int)(object)theItem.Item).ToString();

			} else if (theItem.ThisType.Equals(typeof(char[]))) {
				Entry ent = settingHolder as Entry;
				int cnt = 0;
				foreach (char ch in (theItem.Item as char[])) {
					if (cnt > 0) {
						ent.Text += ";";	
					}
					if (ch == '\t') {
						ent.Text += "\\t";
					} else {
						ent.Text += ch.ToString ();
						cnt++;
					}
				}
			} else if (theItem.ThisType.Equals(typeof(double))) {
				Entry ent = settingHolder as Entry;
				ent.Text = ((double)(object)theItem.Item).ToString ();
			}
		}

		protected bool ConvertContentToType()
		{
			bool success = false;

			if (theItem.ThisType == typeof(string)) {
				Entry ent = settingHolder as Entry;
				if (ent.Text.Length > 0) {
					theItem.Item = (T)(object)ent.Text;
					success = true;
				}

			} else if (theItem.ThisType ==  typeof(bool)) {
				CheckButton chk = settingHolder as CheckButton;
				theItem.Item = (T)(object)chk.Active;
				success = true;

			} else if (theItem.ThisType == typeof(List<string>)) {
				Entry ent = settingHolder as Entry;
				string[] strs = ent.Text.Split (new char [] { ';' }, 10, StringSplitOptions.RemoveEmptyEntries);
				List<string> lstrs = new List<string> ();
				foreach (string st in strs) {
					lstrs.Add (st);
				}
				if (lstrs.Count > 0) {
					theItem.Item = (T)(object)lstrs;
					success = true;
				}

			} else if (theItem.ThisType == typeof(int)) {
				Entry ent = settingHolder as Entry;
				int res;
				if (int.TryParse (ent.Text, out res)) {
					theItem.Item = (T)(object) res;
					success = true;
				}

			} else if (theItem.ThisType == typeof(char[])) {
				Entry ent = settingHolder as Entry;
				string str = ent.Text;
				List<char> lchrs = new List<char> ();
				if (str.Contains ("\\t")) {
					str.Replace ("\\t", "\t");
				}
				foreach (string st in str.Split (new char [] { ';' }, 10, StringSplitOptions.RemoveEmptyEntries)) {
					if (st.Length == 1) {
						lchrs.Add (st.ToCharArray () [0]);
					}
				}
				if (lchrs.Count > 0) {
					theItem.Item = (T)(object)lchrs.ToArray ();
					success = true;
				}

			} else if (theItem.ThisType == typeof(double)) {
				Entry ent = settingHolder as Entry;
				double dub;
				if (double.TryParse (ent.Text, out dub)) {
					theItem.Item = (T)(object) dub;
					success = true;
				}

			} 
			return success;
		}

		public void AddDependent(Widget wid)
		{
			if (dependents == null) {
				dependents = new List<Widget> ();
			}
			dependents.Add (wid);
		}
			
	}
}

