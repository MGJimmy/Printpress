export interface PromptDialogModel {
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
  fieldLabel: string;
  maxLength?: number;
}
