using System;
using System.Runtime.InteropServices;
using static anoncreds_rs_dotnet.Models.Structures;

namespace anoncreds_rs_dotnet.Anoncreds
{
    internal static class NativeMethods
    {
        #region Error
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_get_current_error(ref string errorJson);
        #endregion

        #region Mod
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_set_default_logger();
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern string anoncreds_version();
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_buffer_free(ByteBuffer byteBuffer);
        #endregion

        #region PresReq
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_generate_nonce(ref string nonce);
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_presentation_request_from_json(ByteBuffer presentationRequestJson, ref IntPtr presentationRequestObjectHandle);
        #endregion

        #region CredentialDefinition
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_credential_definition(FfiStr schemaId, IntPtr schemaObjectHandle, FfiStr tag, FfiStr issuerId, FfiStr signatureType, byte supportRevocation, ref IntPtr credDefObjectHandle, ref IntPtr credDefPvtObjectHandle, ref IntPtr keyProofObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_definition_from_json(ByteBuffer credDefJson, ref IntPtr credDefObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_definition_private_from_json(ByteBuffer credDefJson, ref IntPtr credDefPrivObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_key_correctness_proof_from_json(ByteBuffer keyCorrectnessProofJson, ref IntPtr keyCorrectnessProofObjectHandle);
        #endregion

        #region CredentialOffer 
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_credential_offer(FfiStr schemaId, FfiStr credDefId, IntPtr keyProofObjectHandle, ref IntPtr credOfferHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_offer_from_json(ByteBuffer credOfferJson, ref IntPtr credOfferObjectHandle);
        #endregion

        #region CredentialRequest
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_credential_request(FfiStr entropy, FfiStr proverDid, IntPtr credDefObjectHandle, FfiStr linkSecret, FfiStr linkSecretId, IntPtr credOfferObjectHandle, ref IntPtr credReqObjectHandle, ref IntPtr credReqMetaObjectHandle);
        
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_request_from_json(ByteBuffer credReqJson, ref IntPtr credReqObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_request_metadata_from_json(ByteBuffer credReqMetaJson, ref IntPtr credReqMetaObjectHandle);
        #endregion

        #region Credential
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_credential(
            IntPtr credDefObjectHandle,
            IntPtr credDefPrivateObjectHandle,
            IntPtr credOfferObjectHandle,
            IntPtr credRequestObjectHandle,
            FfiStrList attrNames,
            FfiStrList attrRawValues,
            FfiStrList attrEncValues,
            FfiStr revRegId,
            IntPtr revStatusListObjectHandle,
            FfiCredRevInfo revocation,
            ref IntPtr credObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_credential(
            IntPtr credDefObjectHandle,
            IntPtr credDefPrivateObjectHandle,
            IntPtr credOfferObjectHandle,
            IntPtr credRequestObjectHandle,
            FfiStrList attrNames,
            FfiStrList attrRawValues,
            FfiStrList attrEncValues,
            FfiStr revRegId,
            IntPtr revStatusListObjectHandle,
            IntPtr revocation,
            ref IntPtr credObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_encode_credential_attributes(FfiStrList attrRawValues, ref string result);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_process_credential(
            IntPtr credObjectHandle, 
            IntPtr credReqObjectHandle, 
            FfiStr linkSecret, 
            IntPtr credDefObjectHandle, 
            IntPtr revRegDefObjectHandle, 
            ref IntPtr newCredObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_from_json(ByteBuffer credJson, ref IntPtr credObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_credential_get_attribute(IntPtr credObjectHandle, FfiStr attributeName, ref string result);
        #endregion

        #region LinkSecret
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_link_secret(ref string linkSecretObjectHandle);
        #endregion

        #region Presentation
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_presentation(
            IntPtr presReqObjectHandle,
            FfiCredentialEntryList credentials,
            FfiCredentialProveList credentialsProve,
            FfiStrList selfAttestNames,
            FfiStrList selfAttestValues,
            FfiStr linkSecret,
            FfiUIntList schemas,
            FfiStrList schemaIds,
            FfiUIntList credDefs,
            FfiStrList credDefIds,
            ref IntPtr presentationObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_presentation_from_json(
            ByteBuffer presentationJson, 
            ref IntPtr presentationObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_verify_presentation(
            IntPtr presObjectHandle,
            IntPtr presReqObjectHandle,
            FfiUIntList schemaObjectHandles,
            FfiStrList schemaIds,
            FfiUIntList credDefObjectHandles,
            FfiStrList credDefIds,
            FfiUIntList revRegDefObjectHandles,
            FfiStrList revRegDefIds,
            FfiUIntList revStatusObjectHandles,
            FfiNonrevokedIntervalOverrideList ffiNonrevokedIntervalOverride,
            ref byte verifyResult);
        #endregion

        #region Revocation
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_revocation_status_list(
            FfiStr revRegDefId,
            IntPtr revRegDefObjectHandle,
            FfiStr issuerId,
            long timestamp,
            byte issuanceByDefault,
            ref IntPtr revStatusListObjectHandle);
        
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_update_revocation_status_list(
            long timestamp,
            FfiLongList issued,
            FfiLongList revoked,
            IntPtr revRegDefObjectHandle,
            IntPtr currentRevStatusListObjectHandle,
            ref IntPtr newRevStatusListObjectHandle);
        
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
         internal static extern int anoncreds_update_revocation_status_list_timestamp_only(
            long timestamp,
            IntPtr currentRevStatusListObjectHandle,
            ref IntPtr newRevStatusListObjectHandle);

        
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_revocation_registry_def(
            IntPtr credDefObjectHandle,
            FfiStr credDefId,
            FfiStr originDid,
            FfiStr tag,
            FfiStr revRegType,
            long maxCredNumber,
            FfiStr tailsDirPath,
            ref IntPtr revRegDefObjectHandle,
            ref IntPtr revRegDefPvtObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_revocation_registry_definition_from_json(
            ByteBuffer revRegJson, 
            ref IntPtr revRegObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_revocation_registry_definition_get_attribute(
            IntPtr regDefObjectHandle,
            FfiStr attributeName,
            ref string result);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_revocation_registry_definition_private_from_json(
            ByteBuffer revRegDefPvtJson, 
            ref IntPtr revRegDefPvtObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_revocation_registry_from_json(
            ByteBuffer revRegJson, 
            ref IntPtr revRegObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_revocation_status_list_from_json(
            ByteBuffer revocationStatusListJson, 
            ref IntPtr revocationStatusListObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_create_or_update_revocation_state(
            IntPtr revRegDefObjectHandle,
            IntPtr revStatusListObjectHandle,
            long revRegIndex,
            FfiStr tailsPath,
            IntPtr currentRevStateObjectHandle,
            IntPtr oldRevStatusListObjectHandle,
            ref IntPtr newRevStateObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_revocation_state_from_json(ByteBuffer credentialRevocationStateJson, ref IntPtr credentialRevocationStateObjectHandle);
        #endregion

        #region Schema
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int anoncreds_create_schema(FfiStr schemaName, FfiStr schemaVersion, FfiStr issuerId, FfiStrList attrNames, ref IntPtr schemaObjectHandle);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_schema_from_json(ByteBuffer schemaJson, ref IntPtr schemaObjectHandle);
        #endregion

        #region ObjectHandle
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_object_get_type_name(IntPtr objectHandle, ref string result);

        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int anoncreds_object_get_json(IntPtr objectHandle, ref ByteBuffer result);
        [DllImport(Consts.CREDX_LIB_NAME, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern void anoncreds_object_free(IntPtr objectHandle);
        #endregion
    }
}