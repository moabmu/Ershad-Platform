using System;
using System.Reflection;
using System.Linq.Expressions;

namespace ModepEduYanbu.Helpers.Common
{
    public static class ObjectCreation
    {
        public delegate T Creator<T>(params object[] args);

        private static Creator<T> GetCreator<T>()
        {
            // Get constructor information?
            ConstructorInfo[] constructors = typeof(T).GetConstructors();

            // Is there at least 1?
            if (constructors.Length >= 0)
            {
                // Get our one constructor.
                ConstructorInfo constructor = constructors[0];

                // Yes, does this constructor take some parameters?
                ParameterInfo[] paramsInfo = constructor.GetParameters();

                if (paramsInfo.Length > 0)
                {
                    // Create a single param of type object[].
                    ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

                    // Pick each arg from the params array and create a typed expression of them.
                    Expression[] argsExpressions = new Expression[paramsInfo.Length];

                    for (int i = 0; i < paramsInfo.Length; i++)
                    {
                        Expression index = Expression.Constant(i);
                        Type paramType = paramsInfo[i].ParameterType;
                        Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                        Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                        argsExpressions[i] = paramCastExp;
                    }

                    // Make a NewExpression that calls the constructor with the args we just created.
                    NewExpression newExpression = Expression.New(constructor, argsExpressions);

                    // Create a lambda with the NewExpression as body and our param object[] as arg.
                    LambdaExpression lambda = Expression.Lambda(typeof(Creator<T>), newExpression, param);

                    // Compile it
                    Creator<T> compiled = (Creator<T>)lambda.Compile();

                    // Success
                    return compiled;
                }
            }

            return null;
        }

        /// <summary>Create instance of T with parameters using ConstructorInfo.Invoke</summary>
        public static T CreateInstanceUsingLamdaExpression<T>(params object[] args)
        {
            Creator<T> createdActivator = GetCreator<T>();
            return createdActivator(args);
        }

        /// <summary>
        /// An delegate which invoke the object's property setter through reflection
        /// </summary>
        // https://elvincheng.wordpress.com/2010/12/15/how-do-i-set-a-fieldproperty-value-in-an-c-expression-tree-in-net-3-5/
        public static Action<TElement, TValue> GetPropertySetter<TElement, TValue>(TElement elem, string propertyName)
        {
            Type elementType = elem.GetType();

            PropertyInfo pi = elementType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            MethodInfo mi = pi.GetSetMethod();  //  This retrieves the 'get_LastName' method

            ParameterExpression oParam = Expression.Parameter(elementType, "obj");
            ParameterExpression vParam = Expression.Parameter(typeof(TValue), "val");
            MethodCallExpression mce = Expression.Call(oParam, mi, vParam);
            Expression<Action<TElement, TValue>> action = Expression.Lambda<Action<TElement, TValue>>(mce, oParam, vParam);

            return action.Compile();
        }

        //static Action<TElement, TValue> GetPropertySetter<TElement, TValue>(TElement elem, string propertyName)
        //{
        //    Type elementType = elem.GetType();

        //    PropertyInfo pi = elementType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        //    MethodInfo mi = pi.GetSetMethod();  //  This retrieves the 'get_LastName' method

        //    ParameterExpression oParam = Expression.Parameter(elementType, "obj");
        //    ParameterExpression vParam = Expression.Parameter(typeof(TValue), "val");
        //    MethodCallExpression mce = Expression.Call(oParam, mi, vParam);
        //    Expression<Action<TElement, TValue>> action = Expression.Lambda<Action<TElement, TValue>>(mce, oParam, vParam);

        //    return action.Compile();
        //}
    }
}