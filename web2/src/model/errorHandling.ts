export type Problem = {
  Type: string | null,
  Title: string,
  Status: number,
  Detail: string | null,
  Instance: string | null,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  Extensions: any
};
