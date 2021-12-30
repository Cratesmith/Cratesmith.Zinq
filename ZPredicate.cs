namespace Zinq
{
	public delegate bool ZPredicate<TContext, TSource>(in TContext context, in TSource source);
	public delegate bool ZPredicate<TSource>(in TSource source);
}
