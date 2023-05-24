using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace anoncreds_rs_dotnet.Models
{
    public class PresentationRequest
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        public string JsonString { get; set; }
        [JsonProperty("requested_attributes")]
        public Dictionary<string, AttributeInfo> RequestedAttributes { get; set; }
        [JsonProperty("requested_predicates")]
        public Dictionary<string, PredicateInfo> RequestedPredicates { get; set; }
        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
        public IntPtr Handle { get; set; }
    }

    internal class QueryRequest
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        public string JsonString { get; set; }
        [JsonProperty("requested_attributes")]
        public Dictionary<string, QueryAttributeInfo> RequestedAttributes { get; set; }
        [JsonProperty("requested_predicates")]
        public Dictionary<string, QueryPredicateInfo> RequestedPredicates { get; set; }
        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
        public IntPtr Handle { get; set; }
    }

    internal class QueryAttributeInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("names")]
        public List<string> Names { get; set; }

        [JsonProperty("restrictions")]
        public OuterRestriction Restrictions { get; set; }

        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
    }

    internal class QueryPredicateInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("p_type")]
        public string PredicateType { get; set; }
        [JsonProperty("p_value")]
        public int PredicateValue { get; set; }
        [JsonProperty("restrictions")]
        public OuterRestriction Restrictions { get; set; }
        [JsonProperty("non_revoked")]
        public NonRevokedInterval NonRevoked { get; set; }
    }

    internal class OuterRestriction
    {
        [JsonProperty("$or")]
        public List<InnerRestriction> Inners { get; set; }
    }

    internal class InnerRestriction
    {

    }

    internal class InnerResWithQuery : InnerRestriction
    {
        [JsonProperty("$and")]
        public List<PartialFilter> PartialFilters { get; set; }
    }

    internal class PartialFilter : InnerRestriction
    {
        [JsonProperty("schema_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaId { get; set; }

        [JsonProperty("schema_issuer_did", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaIssuerDid { get; set; }

        [JsonProperty("schema_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaName { get; set; }

        [JsonProperty("schema_version", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaVersion { get; set; }

        [JsonProperty("issuer_did", NullValueHandling = NullValueHandling.Ignore)]
        public string IssuerDid { get; set; }

        [JsonProperty("cred_def_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CredentialDefinitionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AttributeValue AttributeValue { get; set; }
    }

    public static class RequestExtensions
    {
        internal static PresentationRequest ToPresentationRequest(this QueryRequest request)
        {
            Dictionary<string, AttributeInfo> reqAttrs = new Dictionary<string, AttributeInfo>();

            if(request.RequestedAttributes != null)
            {
                foreach (KeyValuePair<string, QueryAttributeInfo> keyValuePair in request.RequestedAttributes)
                {
                    string key = keyValuePair.Key;
                    QueryAttributeInfo queryAttributeInfo = keyValuePair.Value;
                    List<AttributeFilter> attributeFilters = new List<AttributeFilter>();

                    foreach (InnerRestriction innerRestriction in queryAttributeInfo.Restrictions.Inners)
                    {
                        if (innerRestriction is PartialFilter partialFilter)
                        {
                            AttributeFilter newFilter = new AttributeFilter();
                            newFilter.FromPartialFilters(new List<PartialFilter>() { partialFilter });

                            attributeFilters.Add(newFilter);
                        }
                        else if (innerRestriction is InnerResWithQuery innerResWithQuery)
                        {
                            AttributeFilter newFilter = new AttributeFilter();
                            newFilter.FromPartialFilters(innerResWithQuery.PartialFilters);

                            attributeFilters.Add(newFilter);
                        }
                    }

                    AttributeInfo attributeInfo = new AttributeInfo()
                    {
                        Name = queryAttributeInfo.Name,
                        Names = queryAttributeInfo.Names,
                        NonRevoked = queryAttributeInfo.NonRevoked,
                        Restrictions = attributeFilters
                    };

                    reqAttrs.Add(key, attributeInfo);
                }
            }

            Dictionary<string, PredicateInfo> reqPreds = new Dictionary<string, PredicateInfo>();

            if(request.RequestedPredicates != null)
            {
                foreach (KeyValuePair<string, QueryPredicateInfo> keyValuePair in request.RequestedPredicates)
                {
                    string key = keyValuePair.Key;
                    QueryPredicateInfo queryPredicateInfo = keyValuePair.Value;
                    List<AttributeFilter> attributeFilters = new List<AttributeFilter>();

                    foreach (InnerRestriction innerRestriction in queryPredicateInfo.Restrictions.Inners)
                    {
                        if (innerRestriction is PartialFilter partialFilter)
                        {
                            AttributeFilter newFilter = new AttributeFilter();
                            newFilter.FromPartialFilters(new List<PartialFilter>() { partialFilter });

                            attributeFilters.Add(newFilter);
                        }
                        else if (innerRestriction is InnerResWithQuery innerResWithQuery)
                        {
                            AttributeFilter newFilter = new AttributeFilter();
                            newFilter.FromPartialFilters(innerResWithQuery.PartialFilters);

                            attributeFilters.Add(newFilter);
                        }
                    }

                    PredicateInfo predicateInfo = new PredicateInfo()
                    {
                        Name = queryPredicateInfo.Name,
                        NonRevoked = queryPredicateInfo.NonRevoked,
                        Restrictions = attributeFilters,
                        PredicateType = queryPredicateInfo.PredicateType,
                        PredicateValue = queryPredicateInfo.PredicateValue
                    };

                    reqPreds.Add(key, predicateInfo);
                }
            }

            PresentationRequest presentationRequest = new PresentationRequest
            {
                Nonce = request.Nonce,
                Name = request.Name,
                Version = request.Version,
                JsonString = request.JsonString,
                NonRevoked = request.NonRevoked,
                Handle = request.Handle,
                RequestedAttributes = reqAttrs,
                RequestedPredicates = reqPreds
            };

            return presentationRequest;
        }

        public static string ToQueryRequestJson(this PresentationRequest presentationRequest)
        {
            QueryRequest queryRequest = new QueryRequest();
            Dictionary<string, QueryAttributeInfo> reqAttrs = new Dictionary<string, QueryAttributeInfo>();
            Dictionary<string, QueryPredicateInfo> reqPreds = new Dictionary<string, QueryPredicateInfo>();

            if(presentationRequest.RequestedAttributes != null)
            {
                foreach (KeyValuePair<string, AttributeInfo> keyValuePair in presentationRequest.RequestedAttributes)
                {
                    AttributeInfo attributeInfo = keyValuePair.Value;
                    OuterRestriction outerRestriction = new OuterRestriction() { Inners = new List<InnerRestriction>() };

                    if(keyValuePair.Value.Restrictions != null)
                    {
                        foreach (AttributeFilter attributeFilter in keyValuePair.Value.Restrictions)
                        {
                            List<PartialFilter> partialFilters = new List<PartialFilter>();

                            if (attributeFilter.SchemaId != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaId = attributeFilter.SchemaId });
                            }
                            if (attributeFilter.SchemaIssuerDid != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaIssuerDid = attributeFilter.SchemaIssuerDid });
                            }
                            if (attributeFilter.SchemaName != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaName = attributeFilter.SchemaName });
                            }
                            if (attributeFilter.SchemaVersion != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaVersion = attributeFilter.SchemaVersion });
                            }
                            if (attributeFilter.IssuerDid != null)
                            {
                                partialFilters.Add(new PartialFilter { IssuerDid = attributeFilter.IssuerDid });
                            }
                            if (attributeFilter.CredentialDefinitionId != null)
                            {
                                partialFilters.Add(new PartialFilter { CredentialDefinitionId = attributeFilter.CredentialDefinitionId });
                            }
                            if (attributeFilter.AttributeValue != null)
                            {
                                partialFilters.Add(new PartialFilter { AttributeValue = attributeFilter.AttributeValue });
                            }

                            if (partialFilters.Count > 1)
                            {
                                outerRestriction.Inners.Add(new InnerResWithQuery { PartialFilters = partialFilters });
                            }
                            else if (partialFilters.Count == 1)
                            {
                                outerRestriction.Inners.Add(partialFilters.First());
                            }
                        }
                    }

                    QueryAttributeInfo queryAttributeInfo = new QueryAttributeInfo()
                    {
                        Name = attributeInfo.Name,
                        Names = attributeInfo.Names,
                        NonRevoked = attributeInfo.NonRevoked,
                        Restrictions = outerRestriction
                    };

                    reqAttrs.Add(keyValuePair.Key, queryAttributeInfo);
                }
            }

            if(presentationRequest.RequestedPredicates != null)
            {
                foreach (KeyValuePair<string, PredicateInfo> keyValuePair in presentationRequest.RequestedPredicates)
                {
                    PredicateInfo predicateInfo = keyValuePair.Value;
                    OuterRestriction outerRestriction = new OuterRestriction() { Inners = new List<InnerRestriction>() };

                    if(keyValuePair.Value.Restrictions != null)
                    {
                        foreach (AttributeFilter attributeFilter in keyValuePair.Value.Restrictions)
                        {
                            List<PartialFilter> partialFilters = new List<PartialFilter>();

                            if (attributeFilter.SchemaId != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaId = attributeFilter.SchemaId });
                            }
                            if (attributeFilter.SchemaIssuerDid != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaIssuerDid = attributeFilter.SchemaIssuerDid });
                            }
                            if (attributeFilter.SchemaName != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaName = attributeFilter.SchemaName });
                            }
                            if (attributeFilter.SchemaVersion != null)
                            {
                                partialFilters.Add(new PartialFilter { SchemaVersion = attributeFilter.SchemaVersion });
                            }
                            if (attributeFilter.IssuerDid != null)
                            {
                                partialFilters.Add(new PartialFilter { IssuerDid = attributeFilter.IssuerDid });
                            }
                            if (attributeFilter.CredentialDefinitionId != null)
                            {
                                partialFilters.Add(new PartialFilter { CredentialDefinitionId = attributeFilter.CredentialDefinitionId });
                            }
                            if (attributeFilter.AttributeValue != null)
                            {
                                partialFilters.Add(new PartialFilter { AttributeValue = attributeFilter.AttributeValue });
                            }

                            if (partialFilters.Count > 1)
                            {
                                outerRestriction.Inners.Add(new InnerResWithQuery { PartialFilters = partialFilters });
                            }
                            else if (partialFilters.Count == 1)
                            {
                                outerRestriction.Inners.Add(partialFilters.First());
                            }
                        }
                    }

                    QueryPredicateInfo queryPredicateInfo = new QueryPredicateInfo()
                    {
                        Name = predicateInfo.Name,
                        NonRevoked = predicateInfo.NonRevoked,
                        Restrictions = outerRestriction,
                        PredicateType = predicateInfo.PredicateType,
                        PredicateValue = predicateInfo.PredicateValue
                    };

                    reqPreds.Add(keyValuePair.Key, queryPredicateInfo);
                }
            }

            queryRequest.Nonce = presentationRequest.Nonce;
            queryRequest.Name = presentationRequest.Name;
            queryRequest.Version = presentationRequest.Version;
            queryRequest.JsonString = presentationRequest.JsonString;
            queryRequest.NonRevoked = presentationRequest.NonRevoked;
            queryRequest.Handle = presentationRequest.Handle;
            queryRequest.RequestedAttributes = reqAttrs;
            queryRequest.RequestedPredicates = reqPreds;

            return JsonConvert.SerializeObject(queryRequest);
        }

        internal static void FromPartialFilters(this AttributeFilter filter, List<PartialFilter> partialFilters)
        {
            foreach (PartialFilter partialFilter in partialFilters)
            {
                if (partialFilter.SchemaId != null && filter.SchemaId == null)
                {
                    filter.SchemaId = partialFilter.SchemaId;
                }
                if (partialFilter.SchemaIssuerDid != null && filter.SchemaIssuerDid == null)
                {
                    filter.SchemaIssuerDid = partialFilter.SchemaIssuerDid;
                }
                if (partialFilter.SchemaName != null && filter.SchemaName == null)
                {
                    filter.SchemaName = partialFilter.SchemaName;
                }
                if (partialFilter.SchemaVersion != null && filter.SchemaVersion == null)
                {
                    filter.SchemaVersion = partialFilter.SchemaVersion;
                }
                if (partialFilter.IssuerDid != null && filter.IssuerDid == null)
                {
                    filter.IssuerDid = partialFilter.IssuerDid;
                }
                if (partialFilter.CredentialDefinitionId != null && filter.CredentialDefinitionId == null)
                {
                    filter.CredentialDefinitionId = partialFilter.CredentialDefinitionId;
                }
                if (partialFilter.AttributeValue != null && filter.AttributeValue == null)
                {
                    filter.AttributeValue = partialFilter.AttributeValue;
                }
            }
        }

        internal static List<PartialFilter> ToPartialFilters(this AttributeFilter filter)
        {
            List<PartialFilter> partialFilters = new List<PartialFilter>();

            if (filter.SchemaId != null)
            {
                partialFilters.Add(new PartialFilter { SchemaId = filter.SchemaId });
            }
            if (filter.SchemaName != null)
            {
                partialFilters.Add(new PartialFilter { SchemaName = filter.SchemaName });
            }
            if (filter.SchemaIssuerDid != null)
            {
                partialFilters.Add(new PartialFilter { SchemaIssuerDid = filter.SchemaIssuerDid });
            }
            if (filter.SchemaVersion != null)
            {
                partialFilters.Add(new PartialFilter { SchemaVersion = filter.SchemaVersion });
            }
            if (filter.IssuerDid != null)
            {
                partialFilters.Add(new PartialFilter { IssuerDid = filter.IssuerDid });
            }
            if (filter.CredentialDefinitionId != null)
            {
                partialFilters.Add(new PartialFilter { CredentialDefinitionId = filter.CredentialDefinitionId });
            }
            if (filter.AttributeValue != null)
            {
                partialFilters.Add(new PartialFilter { AttributeValue = filter.AttributeValue });
            }

            return partialFilters;
        }

        internal static QueryRequest ToQueryRequest(this string queryRequestJson)
        {
            QueryRequest request = JsonConvert.DeserializeObject<QueryRequest>(queryRequestJson, Settings.JsonSettings);
            request.RequestedAttributes = new Dictionary<string, QueryAttributeInfo>();
            request.RequestedPredicates = new Dictionary<string, QueryPredicateInfo>();

            JObject requestJObject = JObject.Parse(queryRequestJson);
            JObject attrJObject = JObject.Parse(requestJObject["requested_attributes"].ToString());
            foreach(var keyValuePair in attrJObject)
            {
                QueryAttributeInfo queryAttributeInfo = JsonConvert.DeserializeObject<QueryAttributeInfo>(keyValuePair.Value.ToString(), Settings.JsonSettings);
                JObject attrInfoJObject = JObject.Parse(keyValuePair.Value.ToString());
                if (attrInfoJObject.ContainsKey("restrictions"))
                {
                    queryAttributeInfo.Restrictions = new OuterRestriction() { Inners = new List<InnerRestriction>() };
                    JObject restrictionsJObject = JObject.Parse(attrInfoJObject["restrictions"].ToString());

                    if (restrictionsJObject.ContainsKey("$or"))
                    {
                        var restrictions = restrictionsJObject["$or"].ToArray();
                        foreach (var token in restrictions)
                        {
                            try
                            {
                                var e = JObject.Parse(token.ToString());
                                if (e.ContainsKey("$and"))
                                {
                                    queryAttributeInfo.Restrictions.Inners.Add(JsonConvert.DeserializeObject<InnerResWithQuery>(token.ToString()));
                                }
                                else
                                {
                                    queryAttributeInfo.Restrictions.Inners.Add(JsonConvert.DeserializeObject<PartialFilter>(token.ToString()));
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                    
                request.RequestedAttributes.Add(keyValuePair.Key, queryAttributeInfo);
            }
            JObject predJObject = JObject.Parse(requestJObject["requested_predicates"].ToString());
            foreach (var keyValuePair in predJObject)
            {
                QueryPredicateInfo queryPredicateInfo = JsonConvert.DeserializeObject<QueryPredicateInfo>(keyValuePair.Value.ToString(), Settings.JsonSettings);
                JObject predInfoJObject = JObject.Parse(keyValuePair.Value.ToString());
                if (predInfoJObject.ContainsKey("restrictions"))
                {
                    queryPredicateInfo.Restrictions = new OuterRestriction() { Inners = new List<InnerRestriction>() };
                    JObject restrictionsJObject = JObject.Parse(predInfoJObject["restrictions"].ToString());

                    if (restrictionsJObject.ContainsKey("$or"))
                    {
                        var restrictions = JObject.Parse(predInfoJObject["restrictions"].ToString())["$or"].ToArray();
                        foreach (var token in restrictions)
                        {
                            try
                            {
                                var e = JObject.Parse(token.ToString());
                                if (e.ContainsKey("$and"))
                                {
                                    queryPredicateInfo.Restrictions.Inners.Add(JsonConvert.DeserializeObject<InnerResWithQuery>(token.ToString()));
                                }
                                else
                                {
                                    queryPredicateInfo.Restrictions.Inners.Add(JsonConvert.DeserializeObject<PartialFilter>(token.ToString()));
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                
                request.RequestedPredicates.Add(keyValuePair.Key, queryPredicateInfo);
            }

            return request;
        }
    }
}