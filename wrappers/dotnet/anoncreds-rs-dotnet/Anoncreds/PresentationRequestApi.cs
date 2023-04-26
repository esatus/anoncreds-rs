using anoncreds_rs_dotnet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    public static class PresentationRequestApi
    {
        /// <summary>
        /// Generates a new random nonce.
        /// </summary>
        /// <exception cref="AnoncredsRsException">Throws when nonce can not be generated.</exception>
        /// <returns>New nonce.</returns>
        public static async Task<string> GenerateNonceAsync()
        {
            string result = "";
            int errorCode = NativeMethods.anoncreds_generate_nonce(ref result);
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Creates a new <see cref="PresentationRequest"/> object to a provided JSON string.
        /// </summary>
        /// <param name="presReqJson">JSON string of a presentation request object.</param>
        /// <exception cref="AnoncredsRsException">Throws when <paramref name="presReqJson"/> is invalid.</exception>
        /// <exception cref="IndexOutOfRangeException">Throws when <paramref name="presReqJson"/> is empty.</exception>
        /// <returns>A new <see cref="PresentationRequest"/>.</returns>
        public static async Task<PresentationRequest> CreatePresReqFromJsonAsync(string presReqJson)
        {
            IntPtr presReqObjectHandle = new IntPtr();
            int errorCode = NativeMethods.anoncreds_presentation_request_from_json(ByteBuffer.Create(presReqJson), ref presReqObjectHandle);
            if (errorCode != 0)
            {
                string error = await ErrorApi.GetCurrentErrorAsync();
                throw AnoncredsRsException.FromSdkError(error);
            }
            PresentationRequest presReq = await CreatePresentationRequestObject(presReqObjectHandle);
            return await Task.FromResult(presReq);
        }

        /// <summary>
        /// Creates a new <see cref="PresentationRequest"/> object to a provided handle.
        /// </summary>
        /// <param name="objectHandle">Handle of a presentation request object.</param>
        /// <returns>A new <see cref="PresentationRequest"/>.</returns>
        private static async Task<PresentationRequest> CreatePresentationRequestObject(IntPtr objectHandle)
        {
            string presReqJson = await ObjectApi.ToJsonAsync(objectHandle);
            PresentationRequest presentationRequestObject = JsonConvert.DeserializeObject<PresentationRequest>(presReqJson, Settings.JsonSettings);
            presentationRequestObject.JsonString = presReqJson;

            presentationRequestObject.RequestedAttributes = new Dictionary<string, AttributeInfo>();
            presentationRequestObject.RequestedPredicates = new Dictionary<string, PredicateInfo>();

            JObject presReqJObject = JObject.Parse(presReqJson);

            JToken requestedAttributes = presReqJObject["requested_attributes"];
            foreach (JToken attribute in requestedAttributes)
            {
                string key = attribute.Path.Split('.')[1];
                foreach (JToken element in attribute)
                {
                    try
                    {
                        AttributeInfo info = new AttributeInfo()
                        {
                            Name = element["name"].Value<string>()
                        };
                        if (element["names"] != null)
                        {
                            info.Names = element["names"].ToObject<List<string>>();
                        }
                        info.Restrictions = CreateAttributeFilterList(element["restrictions"]);
                        info.NonRevoked = element["non_revoked"].ToObject<NonRevokedInterval>(); ;
                        presentationRequestObject.RequestedAttributes.Add(key, info);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            JToken requestedPredicates = presReqJObject["requested_predicates"];
            foreach (JToken predicate in requestedPredicates)
            {
                string key = predicate.Path.Split('.')[1];
                foreach (JToken element in predicate)
                {
                    try
                    {
                        PredicateInfo info = new PredicateInfo()
                        {
                            Name = element["name"].Value<string>(),
                            PredicateType = ParsePredicateType(element["p_type"].Value<string>()),
                            PredicateValue = element["p_value"].Value<int>()
                        };
                        if (element["non_revoked"] != null)
                        {
                            info.NonRevoked = element["non_revoked"].ToObject<NonRevokedInterval>();
                        }
                        info.Restrictions = CreateAttributeFilterList(element["restrictions"]);
                        presentationRequestObject.RequestedPredicates.Add(key, info);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            presentationRequestObject.Handle = objectHandle;
            return await Task.FromResult(presentationRequestObject);
        }

        /// <summary>
        /// Creates restrictions from a <see cref="JToken"/>.
        /// </summary>
        /// <param name="restrictionsElement">Restrictions as JToken.</param>
        /// <returns>A list of <see cref="AttributeFilter"/>.</returns>
        private static List<AttributeFilter> CreateAttributeFilterList(JToken restrictionsElement)
        {
            List<AttributeFilter> filterList = new List<AttributeFilter>();
            if (restrictionsElement.HasValues)
            {
                foreach (JObject restriction in restrictionsElement["$or"].Children<JObject>())
                {
                    IEnumerable<JProperty> properties = restriction.ToString().Contains("$and") ? (IEnumerable<JProperty>)restriction["$and"].Children<JObject>().Properties() : restriction.Properties();
                    string filterJson = "{";
                    foreach (JProperty res in properties)
                    {
                        filterJson += "\"" + res.Name + "\": \"" + res.Value.ToString() + "\",";
                    }
                    filterJson += "}";


                    AttributeFilter filter = JsonConvert.DeserializeObject<AttributeFilter>(filterJson);
                    // Only add filter, if at least one property is not null.
                    if (filter.GetType().GetProperties().Select(prop => prop.GetValue(filter)).Any(value => value != null))
                    {
                        filterList.Add(filter);
                    }
                }
            }
            return filterList;
        }

        /// <summary>
        /// Parses the <see cref="PredicateTypes"/>.
        /// </summary>
        /// <param name="type">The predicate type as string.</param>
        /// <returns>The <see cref="PredicateTypes"/>.</returns>
        private static PredicateTypes ParsePredicateType(string type)
        {
            switch (type)
            {
                case "<":
                    return PredicateTypes.LT;
                case "<=":
                    return PredicateTypes.LE;
                case ">":
                    return PredicateTypes.GT;
                default:
                    return PredicateTypes.GE;
            }
        }
    }
}