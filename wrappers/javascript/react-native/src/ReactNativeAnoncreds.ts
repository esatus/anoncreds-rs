import type {
  Anoncreds,
  NativeCredentialEntry,
  NativeCredentialProve,
  NativeCredentialRevocationConfig,
  NativeRevocationEntry,
} from 'anoncreds-shared'

import { ObjectHandle } from 'anoncreds-shared'

import { anoncredsReactNative } from './library'
import { serializeArguments } from './utils'

export class ReactNativeAnoncreds implements Anoncreds {
  public version(): string {
    return anoncredsReactNative.version({})
  }

  public getCurrentError(): string {
    return anoncredsReactNative.getCurrentError({})
  }

  public generateNonce(): string {
    return anoncredsReactNative.generateNonce({})
  }

  public createSchema(options: {
    originDid: string
    name: string
    version: string
    attributeNames: string[]
    sequenceNumber?: number
  }): ObjectHandle {
    const handle = anoncredsReactNative.createSchema(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public createCredentialDefinition(options: {
    originDid: string
    schema: ObjectHandle
    tag: string
    signatureType: string
    supportRevocation: boolean
  }): { credentialDefinition: ObjectHandle; credentialDefinitionPrivate: ObjectHandle; keyProof: ObjectHandle } {
    const { keyProof, credentialDefinition, credentialDefinitionPrivate } =
      anoncredsReactNative.createCredentialDefinition(serializeArguments(options))

    return {
      credentialDefinitionPrivate: new ObjectHandle(credentialDefinitionPrivate),
      credentialDefinition: new ObjectHandle(credentialDefinition),
      keyProof: new ObjectHandle(keyProof),
    }
  }

  public createCredential(options: {
    credentialDefinition: ObjectHandle
    credentialDefinitionPrivate: ObjectHandle
    credentialOffer: ObjectHandle
    credentialRequest: ObjectHandle
    attributeRawValues: Record<string, string>
    attributeEncodedValues?: Record<string, string>
    revocationConfiguration?: NativeCredentialRevocationConfig
  }): { credential: ObjectHandle; revocationRegistry: ObjectHandle; revocationDelta: ObjectHandle } {
    const { credential, revocationDelta, revocationRegistry } = anoncredsReactNative.createCredential(
      serializeArguments(options)
    )

    return {
      revocationRegistry: new ObjectHandle(revocationRegistry),
      credential: new ObjectHandle(credential),
      revocationDelta: new ObjectHandle(revocationDelta),
    }
  }

  public encodeCredentialAttributes(options: { attributeRawValues: Array<string> }): Array<string> {
    const s = anoncredsReactNative.encodeCredentialAttributes(serializeArguments(options))
    return s.split(',')
  }

  public processCredential(options: {
    credential: ObjectHandle
    credentialRequestMetadata: ObjectHandle
    masterSecret: ObjectHandle
    credentialDefinition: ObjectHandle
    revocationRegistryDefinition?: ObjectHandle
  }): ObjectHandle {
    const handle = anoncredsReactNative.processCredential(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public revokeCredential(options: {
    revocationRegistryDefinition: ObjectHandle
    revocationRegistry: ObjectHandle
    credentialRevocationIndex: number
    tailsPath: string
  }): { revocationRegistry: ObjectHandle; revocationRegistryDelta: ObjectHandle } {
    const { revocationRegistry, revocationRegistryDelta } = anoncredsReactNative.revokeCredential(
      serializeArguments(options)
    )

    return {
      revocationRegistryDelta: new ObjectHandle(revocationRegistryDelta),
      revocationRegistry: new ObjectHandle(revocationRegistry),
    }
  }

  public createCredentialOffer(options: {
    schemaId: string
    credentialDefinition: ObjectHandle
    keyProof: ObjectHandle
  }): ObjectHandle {
    const handle = anoncredsReactNative.createCredentialOffer(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public createCredentialRequest(options: {
    proverDid: string
    credentialDefinition: ObjectHandle
    masterSecret: ObjectHandle
    masterSecretId: string
    credentialOffer: ObjectHandle
  }): { credentialRequest: ObjectHandle; credentialRequestMeta: ObjectHandle } {
    const { credentialRequest, credentialRequestMeta } = anoncredsReactNative.createCredentialRequest(
      serializeArguments(options)
    )

    return {
      credentialRequestMeta: new ObjectHandle(credentialRequestMeta),
      credentialRequest: new ObjectHandle(credentialRequest),
    }
  }

  public createMasterSecret(): ObjectHandle {
    const handle = anoncredsReactNative.createMasterSecret({})
    return new ObjectHandle(handle)
  }

  public createPresentation(options: {
    presentationRequest: ObjectHandle
    credentials: NativeCredentialEntry[]
    credentialsProve: NativeCredentialProve[]
    selfAttest: Record<string, string>
    masterSecret: ObjectHandle
    schemas: ObjectHandle[]
    credentialDefinitions: ObjectHandle[]
  }): ObjectHandle {
    const handle = anoncredsReactNative.createPresentation(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public verifyPresentation(options: {
    presentation: ObjectHandle
    presentationRequest: ObjectHandle
    schemas: ObjectHandle[]
    credentialDefinitions: ObjectHandle[]
    revocationRegistryDefinitions: ObjectHandle[]
    revocationEntries: NativeRevocationEntry[]
  }): boolean {
    return anoncredsReactNative.verifyPresentation(serializeArguments(options))
  }

  public createRevocationRegistry(options: {
    originDid: string
    credentialDefinition: ObjectHandle
    tag: string
    revocationRegistryType: string
    issuanceType?: string
    maximumCredentialNumber: number
    tailsDirectoryPath?: string
  }): {
    registryDefinition: ObjectHandle
    registryDefinitionPrivate: ObjectHandle
    registryEntry: ObjectHandle
    registryInitDelta: ObjectHandle
  } {
    const { registryEntry, registryInitDelta, registryDefinition, registryDefinitionPrivate } =
      anoncredsReactNative.createRevocationRegistry(serializeArguments(options))

    return {
      registryDefinitionPrivate: new ObjectHandle(registryDefinitionPrivate),
      registryDefinition: new ObjectHandle(registryDefinition),
      registryInitDelta: new ObjectHandle(registryInitDelta),
      registryEntry: new ObjectHandle(registryEntry),
    }
  }

  public updateRevocationRegistry(options: {
    revocationRegistryDefinition: ObjectHandle
    revocationRegistry: ObjectHandle
    issued: number[]
    revoked: number[]
    tailsDirectoryPath: string
  }): { revocationRegistry: ObjectHandle; revocationRegistryDelta: ObjectHandle } {
    const { revocationRegistry, revocationRegistryDelta } = anoncredsReactNative.updateRevocationRegistry(
      serializeArguments(options)
    )

    return {
      revocationRegistryDelta: new ObjectHandle(revocationRegistryDelta),
      revocationRegistry: new ObjectHandle(revocationRegistry),
    }
  }

  public mergeRevocationRegistryDeltas(options: {
    revocationRegistryDelta1: ObjectHandle
    revocationRegistryDelta2: ObjectHandle
  }): ObjectHandle {
    const handle = anoncredsReactNative.mergeRevocationRegistryDeltas(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public createOrUpdateRevocationState(options: {
    revocationRegistryDefinition: ObjectHandle
    revocationRegistryDelta: ObjectHandle
    revocationRegistryIndex: number
    timestamp: number
    tailsPath: string
    previousRevocationState?: ObjectHandle
  }): ObjectHandle {
    const handle = anoncredsReactNative.createOrUpdateRevocationState(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public presentationRequestFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.presentationRequestFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public schemaGetAttribute(options: { objectHandle: ObjectHandle; name: string }): string {
    return anoncredsReactNative.schemaGetAttribute(serializeArguments(options))
  }

  public revocationRegistryDefinitionGetAttribute(options: { objectHandle: ObjectHandle; name: string }): string {
    return anoncredsReactNative.revocationRegistryDefinitionGetAttribute(serializeArguments(options))
  }

  public credentialGetAttribute(options: { objectHandle: ObjectHandle; name: string }): string {
    return anoncredsReactNative.credentialGetAttribute(serializeArguments(options))
  }

  public getJson(options: { objectHandle: ObjectHandle }): string {
    return anoncredsReactNative.getJson(serializeArguments(options))
  }

  public getTypeName(options: { objectHandle: ObjectHandle }): string {
    return anoncredsReactNative.getTypeName(serializeArguments(options))
  }

  public objectFree(options: { objectHandle: ObjectHandle }): void {
    return anoncredsReactNative.objectFree(serializeArguments(options))
  }

  public credentialDefinitionGetAttribute(options: { objectHandle: ObjectHandle; name: string }): string {
    return anoncredsReactNative.credentialDefinitionGetAttribute(serializeArguments(options))
  }

  public revocationRegistryDefinitionFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.revocationRegistryFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public revocationRegistryFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.revocationRegistryFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public presentationFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.presentationFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public credentialOfferFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.credentialFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public schemaFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.schemaFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public masterSecretFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.masterSecretFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public credentialRequestFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.credentialRequestFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public credentialRequestMetadataFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.credentialRequestMetadataFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public credentialFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.credentialFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public revocationRegistryDefinitionPrivateFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.revocationRegistryDefinitionPrivateFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public revocationRegistryDeltaFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.revocationRegistryDeltaFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public revocationStateFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.revocationStateFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }
  public credentialDefinitionFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.credentialDefinitionFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public credentialDefinitionPrivateFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.credentialDefinitionPrivateFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }

  public keyCorrectnessProofFromJson(options: { json: string }): ObjectHandle {
    const handle = anoncredsReactNative.keyCorrectnessProofFromJson(serializeArguments(options))
    return new ObjectHandle(handle)
  }
}