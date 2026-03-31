export interface ApiResponse<T> {
  success?:    boolean;
  data?:       T;
  message?:    string | null;
  errors?:     string[] | null;
  statusCode?: number;
  timestamp?:  string;
  traceId?:    string | null;
}
