using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Model.DataFlowLogics
{
	public class SpeedTest
	{
		private long _taskPerSecond = 0;

		private long _taskCounter = 0;

		private Task _taskTimer;

		public SpeedTest(int interval = 1000)
		{
			_taskTimer = Task.Run(() =>
			{
				while (true)
				{
					Interlocked.Exchange(ref _taskPerSecond, Interlocked.Read(ref _taskCounter));
					Interlocked.Exchange(ref _taskCounter, 0);
					Thread.Sleep(interval);
				}
			});
		}

		public void Incremental()
		{
			Interlocked.Increment(ref _taskCounter);
		}

		public long GetSpeed()
		{
			return Interlocked.Read(ref _taskPerSecond);
		}
	}
}
