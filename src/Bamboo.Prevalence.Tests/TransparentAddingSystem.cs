// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2002 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net

using System;
using NUnit.Framework;
using Bamboo.Prevalence;
using Bamboo.Prevalence.Attributes;

namespace Bamboo.Prevalence.Tests
{
	/// <summary>
	/// An transparently prevalent class. The methods can be
	/// directly exposed to clients, there's no need to use command
	/// and query objects (they will be created automatically
	/// by the system as needed).
	/// </summary>
	[Serializable]
	[TransparentPrevalence]	
	public class TransparentAddingSystem : System.MarshalByRefObject, IAddingSystem
	{
		private int _total;

		public TransparentAddingSystem()
		{		
		}

		public int Total
		{
			// property accessor are treated as query objects (read lock)
			get
			{
				AssertIsPrevalenceEngineCall();

				return _total;
			}
		}

		// public methods are treated as command objects (write lock)
		// unless the attribute Query has been applied to the method.
		public int Add(int amount)
		{
			AssertIsPrevalenceEngineCall();

			if (amount < 0)
			{
				throw new ArgumentOutOfRangeException("amount", amount, "amount must be positive!");
			}
			_total += amount;
			return _total;
		}

		[PassThrough]
		public void PassThroughMethod()
		{
			Assertion.AssertNull("PassThrough should prevent engine call!", PrevalenceEngine.Current);
		}

		private void AssertIsPrevalenceEngineCall()
		{
			// Just making sure that this call was intercepted and
			// the PrevalenceEngine is available!
			Assertion.AssertNotNull("PrevalenceEngine.Current", PrevalenceEngine.Current);
		}
	}
}