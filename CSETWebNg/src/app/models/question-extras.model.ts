////////////////////////////////
//
//   Copyright 2023 Battelle Energy Alliance, LLC
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
////////////////////////////////
import { Observation } from "../assessment/questions/observations/observations.model";
import { ConfigService } from "../services/config.service";

export interface QuestionExtrasResponse {
  ListTabs: QuestionInformationTabData[];
}

/**
 * Custom documents can ne paired to a single section_Ref or have
 * all of its applicable section_refs grouped in a bookmarks array.
 */
export interface CustomDocument {
  file_Id: number;
  title: string;
  file_Name: string;
  is_Uploaded: boolean;
  section_Ref?: string;
  bookmarks?: string[];
}

/**
 * A document attached to a question answer.
 */
export interface QuestionDocument {
  isEdit?: boolean;
  document_Id: number;
  title: string;
  fileName: string;
}

export interface QuestionDetailsContentViewModel {
  questionId: number;
  selectedStandardTabIndex: number;
  noQuestionInformationText: string;
  showQuestionDetailTab: boolean;
  isDetailAndInfo: boolean;
  isNoQuestion: boolean;
  selectedTabIndex: number;
  listTabs: QuestionInformationTabData[];
  observations: Observation[];
  documents: QuestionDocument[];
}

export interface QuestionInformationTabData {
  requirementFrameworkTitle: String;
  relatedFrameworkCategory: String;
  showRequirementFrameworkTitle: boolean;
  question_or_Requirement_Id: number;
  requirementsData: RequirementTabData;
  additionalDocumentsList: CustomDocument[];
  sourceDocumentsList: CustomDocument[];
  referenceTextList: string[];
  references: string;
  componentTypes: ComponentOverrideLinkInfo[];
  componentVisibility: boolean;
  questionsVisible: boolean;
  showSALLevel: boolean;
  showRequirementStandards: boolean;
  frameworkQuestions: FrameworkQuestionItem[];
  levelName: String;
  isCustomQuestion: boolean;
  isComponent: boolean;
  setsList: string[];
  questionsList: string[];
  showNoQuestionInformation: boolean;
  examinationApproach: String;
  is_Component: boolean;
  is_Maturity: boolean;
}

export interface RequirementTabData {
  text: String;
  supplementalInfo: String;
  set_Name: string;
  examinationApproach: String;
}

export interface ComponentOverrideLinkInfo {
  component_Symbol_Id: number;
  symbol_Name: string;
  enabled: boolean;
}

export interface FrameworkQuestionItem {
  questionText: String;
  standard: String;
  reference: String;
  requirementID: number;
  setName: string;
  categoryAndQuestionNumber: string;
}
