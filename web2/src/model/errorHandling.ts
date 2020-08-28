export type ValidationErrors = {
  /*
   * The structure is dynamic.
   * For each property that failed validation, the errors object has a
   * property of that name. The value of that property is a string array
   * containing error messages.
   */
  [index: string]: string[]
};

export type ValidationProblem = {
  type: string | null,
  title: string,
  status: number,
  traceId: string,
  errors: ValidationErrors
};

export type GeneralProblem = {
  type: string | null,
  title: string,
  statue: number,
  detail: string | null,
  instance: string | null,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  extensions: any
};

export type Problem =
  ValidationProblem |
  GeneralProblem;
