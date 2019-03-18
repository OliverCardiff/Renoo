using System;
using Gtk;
using System.Collections.Generic;

namespace GeneToAnno
{
	public partial class SetEleDictWindow : Gtk.Dialog
	{
		protected ElementTypeDictionary eleTypes;
		protected TypeDictCase uCase;
		protected TypeDictCase dictCase;
		protected List<string> allLabs;
		protected List<string> someLabs;
		protected List<CheckButton> buttons;

		public SetEleDictWindow (ElementTypeDictionary eleType, TypeDictCase useCase)
		{
			uCase = useCase;
			dictCase = useCase;
			eleTypes = eleType;
			buttons = new List<CheckButton> ();
			someLabs = eleTypes.GetTypesforCase (useCase);
			allLabs = eleTypes.GetAllTypes ();
			this.Build ();
			MakeCheckList ();
		}

		protected void MakeCheckList()
		{
			VBox boc = new VBox(true,5);

			foreach (string st in allLabs) {
				CheckButton chk = new CheckButton (st);
				buttons.Add (chk);
				if (someLabs.Contains (st)) {
					chk.Active = true;
				} else {
					chk.Active = false;
				}
				boc.PackEnd (chk, true, false, 5);
			}


			labWindow.AddWithViewport (boc);
			boc.Show ();

			foreach (CheckButton buts in buttons) {
				buts.Show ();
			}
		}

		protected void OnCancel (object sender, EventArgs e)
		{
			Respond (ResponseType.Cancel);
			this.Destroy ();
		}

		protected void OnAccept (object sender, EventArgs e)
		{
			int cnt = allLabs.Count;
			List<string> nextTypeLot = new List<string> ();

			for (int i = 0; i < cnt; i++) {
				if (buttons [i].Active) {
					nextTypeLot.Add (allLabs [i]);
				}
			}

			eleTypes.RecieveTypesForCase (uCase, nextTypeLot);

			Respond (ResponseType.Accept);
			this.Destroy ();
		}
	}
}

