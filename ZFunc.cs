namespace Zinq
{
	public delegate TResult ZFunc<TContext, TSource, TResult>(in TContext context, in TSource source);
	public delegate TResult ZFunc<TSource, TResult>(in TSource source);
}
