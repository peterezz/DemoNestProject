//using iucon.Nest.Extensions.Attributes;
//using iucon.Onix.ElasticSearch.Extensions;
//using Nest;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;

//namespace iucon.Onix.ElasticSearch.CustomVisitors
//{
//    public class CustomPropretyVisitor : NoopPropertyVisitor
//    {
//        private const string Keyword = "keyword";
//        private const string Suggest = "suggest";
//        private const string KeywordNormalizer = "keyword_normalizer";

//        public override void Visit(IBooleanProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(ITokenCountProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(IIpProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(IDateProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(IDateNanosProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(INumberProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(ITextProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//        {
//            type.SetIndexed(false, attribute).SetAnalyzer(attribute);
//            type.Fields = new Properties<object>();
//            if (!type.Fields.Keys.Any(k => k == Keyword))
//                type.Fields.Add(Keyword, new KeywordProperty() { Name = Keyword, IgnoreAbove = 256, Normalizer = KeywordNormalizer });
//            AddCompeletionProperty(propertyInfo, type.Fields);

//        }
//        public override void Visit(IKeywordProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//        {
//            type.SetIndexed(false, attribute);
//            type.Normalizer = KeywordNormalizer;
//            type.Fields = type.Fields ?? new Properties<object>();
//            AddCompeletionProperty(propertyInfo, type.Fields);

//        }
//        public override void Visit(IFlattenedProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override void Visit(ISearchAsYouTypeProperty type, PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//            => type.SetIndexed(false, attribute);
//        public override IProperty Visit(PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//        {
//            IProperty prop;

//            Type t = propertyInfo.PropertyType;
//            if (IsEnumOrNullableEnum(t))
//                prop = OverwriteEnumProperty(propertyInfo, attribute);
//            else if (t.IsGenericType && IsListOfEnumOrNullableEnum(t))
//                prop = OverwriteEnumProperty(propertyInfo, attribute);
//            else if (t.IsArray && IsArrayOfEnumOrNullableEnum(t))
//                prop = OverwriteEnumProperty(propertyInfo, attribute);
//            else
//                prop = base.Visit(propertyInfo, attribute);

//            return prop;
//        }

//        private IProperty OverwriteEnumProperty(PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute)
//        {
//            IProperty prop = new TextProperty();
//            ((TextProperty)prop).Fields = new Properties<string>();
//            Visit((TextProperty)prop, propertyInfo, attribute);
//            return prop;
//        }
//        private static bool IsEnumOrNullableEnum(Type t)
//            => t.IsEnum || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && t.GetGenericArguments()[0].IsEnum);
//        private static bool IsListOfEnumOrNullableEnum(Type t)
//            => t.GetGenericTypeDefinition() == typeof(List<>) && IsEnumOrNullableEnum(t.GetGenericArguments()[0]);
//        private static bool IsArrayOfEnumOrNullableEnum(Type t)
//            => t.IsArray && IsEnumOrNullableEnum(t.GetElementType()!);

//        private void AddCompeletionProperty(PropertyInfo propertyInfo, IProperties propertyContainer)
//        {
//            if (propertyInfo.GetCustomAttribute<SuggestFieldAttribute>() != null)
//                propertyContainer.Add(Suggest, new CompletionProperty() { Name = Suggest });
//        }
//    }
//}
