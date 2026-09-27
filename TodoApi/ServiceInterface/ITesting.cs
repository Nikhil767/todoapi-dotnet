namespace TodoApi.ServiceInterface
{
	public interface ITesting
	{
		void Testing();
	}

	public interface ITest
	{
		void Test();
	}

	public class TestA : ITest
	{
		public void Test()
		{
			throw new NotImplementedException();
		}
	}

	public class TestB : ITest
	{
		public void Test()
		{
			throw new NotImplementedException();
		}
	}

	public class TestC : ITest
	{
		public void Test()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// using FromKeyedServices
	/// </summary>
	public class Testing : ITesting
	{
		private readonly ITest _testA;
		private readonly ITest _testB;
		public Testing([FromKeyedServices("testA")] ITest testA, [FromKeyedServices("testA")] ITest testB)
		{
			_testA = testA;
			_testB = testB;
		}

		void ITesting.Testing()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Using IKeyedServiceProvider
	/// </summary>
	public class Testing2 : ITesting
	{
		private readonly ITest _testA;
		private readonly ITest _testB;

		public Testing2(IKeyedServiceProvider keyedServiceProvider)
		{
			_testA = keyedServiceProvider.GetRequiredKeyedService<ITest>("testA");
			_testB = keyedServiceProvider.GetRequiredKeyedService<ITest>("testB");
		}

		void ITesting.Testing()
		{
			throw new NotImplementedException();
		}
	}
}
