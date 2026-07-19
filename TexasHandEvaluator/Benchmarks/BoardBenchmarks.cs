using BenchmarkDotNet.Attributes;

using Infokom.Gaming.Poker;
using Infokom.Gaming.Poker.Atomics;

using System;
using System.Collections.Generic;
using System.Text;

namespace TexasHandEvaluator.Benchmarks
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser]
	[ThreadingDiagnoser]
	[ExceptionDiagnoser]
	public class BoardBenchmarks
	{
		private readonly Deck _deck = new Deck();
		private readonly Random _rng = Random.Shared;
		private Card[] _cards;
		private CardSpectrum _allCards;

		[GlobalSetup]
		public void Setup()
		{
			 _cards = [.. Enumerable.Repeat(_deck.Draw(_rng), 11)];

			_allCards = CardSpectrum.All;
		}






		//[Benchmark]
		public CardSpectrum CardSpectrum_Create_Benchmark() => CardSpectrum.Create(_cards);

		//[Benchmark]
		public int CardSpectrum_Enumerate52_Benchmark()
		{
			var source = _allCards.GetEnumerator();

			int n = 0;
			while (source.MoveNext())
			{
				_ = source.Current;

				n++;
			}

			return n;

		}

		//[Benchmark]
		public Card[] CardSpectrum_ToArray52_Benchmark()
		{
			var source = _allCards;
			var target = new Card[source.Count];

			Span<Card> span = target;

			int i = 0;
			foreach (var element in source)
			{
				span[i++] = element;
			}

			return target;

		}

		//[Benchmark]
		public CardSequence CardSequence_Create_Benchmark() => CardSequence.Create(_cards);

		[Benchmark]
		public int CardSequence_Enumerate_11_Benchmark()
		{
			var source = CardSequence.Create(Random.Shared, 11).GetEnumerator();

			int n = 0;
			while(source.MoveNext())
			{
				_ = source.Current;

				n++;
			}

			return n;
		}

		//[Benchmark]
		public Card[] CardSequence_ToArray_11_Benchmark()
		{
			var source = _allCards.GetEnumerator();
			var target = new Card[11];

			Span<Card> span = target;

			for (int i = 0; i < 11 && source.MoveNext(); i++)
			{
				span[i] = source.Current;
			}

			return target;

		}

	}
}
