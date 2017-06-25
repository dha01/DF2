namespace SimpleMethods.Simple
{
	public class Math
	{
		/*public static BasicFunction Sum
		{
			get
			{
				var path = new List<string> { "User1" };
				path.AddRange(typeof(Math).FullName.Split('.'));
				path.Add(nameof(Sum));
				return new BasicFunction()
				{
					Id = 1,
					Header = new BasicFunctionHeader()
					{
						Name = nameof(Sum),
						Owners = new List<Owner>(),
						CallStack = path,
						Id = 1
					}
				};
			}
		}*/

		public static int Sum(int item1, int item2)
		{
			return item1 + item2;
		}

		public static int Mul(int item1, int item2)
		{
			return item1 * item2;
		}

		public static bool Greater(int item1, int item2)
		{
			return item1 > item2;
		}

		public static bool Less(int item1, int item2)
		{
			return item1 < item2;
		}

		public static bool Equal(int item1, int item2)
		{
			return item1 == item2;
		}
	}
}
