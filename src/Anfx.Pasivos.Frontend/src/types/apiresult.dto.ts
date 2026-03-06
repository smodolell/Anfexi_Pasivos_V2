export interface ApiResultDto<T> {
    success: boolean;
    message: string;
    errors: string[];
    data: T;
}
