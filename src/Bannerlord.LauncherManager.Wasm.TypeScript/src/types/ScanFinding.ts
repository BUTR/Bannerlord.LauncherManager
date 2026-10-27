export enum Severity {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical',
}

export interface DeveloperGuidance {
  remediation: string;
  documentationUrl: string | null;
  alternativeApis: string[] | null;
  isRemediable: boolean;
}

export enum CallChainNodeType {
  EntryPoint = 'EntryPoint',
  IntermediateCall = 'IntermediateCall',
  SuspiciousDeclaration = 'SuspiciousDeclaration',
}

export interface CallChainNode {
  location: string;
  codeSnippet: string | null;
  description: string;
  nodeType: CallChainNodeType;
}

export interface CallChain {
  chainId: string;
  ruleId: string;
  nodes: CallChainNode[];
  severity: Severity;
  summary: string;
}

export enum DataFlowNodeType {
  Source = 'Source',
  Transform = 'Transform',
  Sink = 'Sink',
  Intermediate = 'Intermediate',
}

export enum DataFlowPattern {
  Legitimate = 'Legitimate',
  DownloadAndExecute = 'DownloadAndExecute',
  DataExfiltration = 'DataExfiltration',
  DynamicCodeLoading = 'DynamicCodeLoading',
  CredentialTheft = 'CredentialTheft',
  RemoteConfigLoad = 'RemoteConfigLoad',
  ObfuscatedPersistence = 'ObfuscatedPersistence',
  Unknown = 'Unknown',
}

export interface DataFlowNode {
  location: string;
  operation: string;
  nodeType: DataFlowNodeType;
  dataDescription: string;
  instructionOffset: number;
  codeSnippet: string | null;
  methodKey: string | null;
  isMethodBoundary: boolean;
  targetMethodKey: string | null;
}

export interface DataFlowChain {
  chainId: string;
  sourceVariable: string | null;
  nodes: DataFlowNode[];
  pattern: DataFlowPattern;
  severity: Severity;
  confidence: number;
  summary: string;
  methodLocation: string;
  isSuspicious: boolean;
  isCrossMethod: boolean;
  involvedMethods: string[];
  callDepth: number;
}

export interface ScanFinding {
  location: string;
  description: string;
  severity: Severity;
  codeSnippet: string | null;
  ruleId: string | null;
  developerGuidance: DeveloperGuidance | null;
  callChain: CallChain | null;
  hasCallChain: boolean;
  dataFlowChain: DataFlowChain | null;
  hasDataFlow: boolean;
  bypassCompanionCheck: boolean;
  riskScore: number | null;
}