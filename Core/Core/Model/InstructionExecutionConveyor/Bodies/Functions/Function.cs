
namespace Core.Model.Bodies.Functions
{
	public enum InputParamCondition
	{
		All,
		Any
	}
	public class Function : Base.ContainerBase, IFunction
	{
		public InputParamCondition Condition { get; set; } = InputParamCondition.All;
	}
}

