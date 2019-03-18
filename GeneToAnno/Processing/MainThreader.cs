using System;
using System.Threading;
using System.Collections.Generic;

namespace GeneToAnno
{
	public abstract class ThreadOutputs
	{

	}
	public class ThreadOutputs<T> : ThreadOutputs
	{
		protected List<T> OutputContainers;

		public ThreadOutputs()
		{
			OutputContainers = new List<T> ();
		}

		public void ReceiveOutput(T output)
		{
			OutputContainers.Add (output);
		}

		public List<T> GetOutputs()
		{
			return OutputContainers;
		}
	}

	public static class MainThreader
	{
		private static ThreadOutputs Outputs;

		private static Action<object> MakeObjConversionAct<T, Tret>(Func<T, Tret> act)
		{
			Action<object> retAct = ob => {
				(Outputs as ThreadOutputs<Tret>).ReceiveOutput(act.Invoke((T)ob));
			};
			return retAct;
		}
		private static Action<object> MakeObjConversionAct<T>(Action<T> act)
		{
			Action<object> retAct = ob => {
				act.Invoke((T)ob);
			};
			return retAct;
		}
		public static List<TReturn> RunDividedList<T, TReturn>(Func<List<T>, TReturn> act, List<T> lst)
		{
			int tmax = AppSettings.Processing.MAX_THREADS.Item;
		
			Outputs = new ThreadOutputs<TReturn> ();

			Action<object> actOb = MakeObjConversionAct<List<T>, TReturn> (act);

			List<List<T>> starters = new List<List<T>> ();
			List<Thread> threadsToUse = new List<Thread> ();
			
			int chunk = lst.Count / tmax;
			int final = lst.Count % tmax;

			for (int i = 0; i < tmax; i++) {
				starters.Add (new List<T> ());
				if (i < tmax - 1) {
					starters [i].AddRange (lst.GetRange (i * chunk, chunk));
				} else {
					starters [i].AddRange (lst.GetRange (i * chunk, chunk + final));
				}
			}

			foreach (List<T> l in starters) {
				Thread th = new Thread (new ParameterizedThreadStart (actOb));
				threadsToUse.Add (th);
			}

			RunAndWait<List<T>> (threadsToUse, starters);

			return (Outputs as ThreadOutputs<TReturn>).GetOutputs ();
		}

		public static void RunDividedList<T>(Action<List<T>> act, List<T> lst)
		{
			int tmax = AppSettings.Processing.MAX_THREADS.Item;

			Action<object> actOb = MakeObjConversionAct<List<T>> (act);

			List<List<T>> starters = new List<List<T>> ();
			List<Thread> threadsToUse = new List<Thread> ();

			int chunk = lst.Count / tmax;
			int final = lst.Count % tmax;

			for (int i = 0; i < tmax; i++) {
				starters.Add (new List<T> ());
				if (i < tmax - 1) {
					starters [i].AddRange (lst.GetRange (i * chunk, chunk));
				} else {
					starters [i].AddRange (lst.GetRange (i * chunk, chunk + final));
				}
			}

			foreach (List<T> l in starters) {
				Thread th = new Thread (new ParameterizedThreadStart (actOb));
				threadsToUse.Add (th);
			}

			RunAndWait<List<T>> (threadsToUse, starters);
		}

		public static List<TReturn> RunOnePerItem<T, TReturn>(Func<T, TReturn> act, List<T> Content)
		{
			Outputs = new ThreadOutputs<TReturn> ();

			Action<object> actOb = MakeObjConversionAct<T, TReturn> (act);

			List<Thread> threadsInUse = new List<Thread> ();
			List<T> contentInUse = new List<T> ();

			foreach (T item in Content) {
				Thread th = new Thread (new ParameterizedThreadStart (actOb));
				threadsInUse.Add (th);
				contentInUse.Add (item);
				if (threadsInUse.Count == AppSettings.Processing.MAX_THREADS.Item) {
					RunAndWait<T> (threadsInUse, contentInUse);
				}
			}
			if (threadsInUse.Count > 0) {
				RunAndWait<T> (threadsInUse, contentInUse);
			}

			return (Outputs as ThreadOutputs<TReturn>).GetOutputs ();
		}

		private static void RunAndWait<T>(List<Thread> lt, List<T> Content)
		{
			int tmax = AppSettings.Processing.MAX_THREADS.Item;
			MainData.UpdateLog ("Running func with " + tmax + " threads...", true);
			int it = 0;

			foreach (Thread t in lt) {
				t.Start((object) Content[it]);
					it++;
			}
			foreach (var thread in lt) {
				thread.Join ();
			}
			lt.Clear ();
			Content.Clear ();
		}
	}
}

