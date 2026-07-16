namespace Infokom.Numerics
{
	public interface IIdentifiable<TSelf>
	{
		public static abstract ulong Id(TSelf source);
	}
}
