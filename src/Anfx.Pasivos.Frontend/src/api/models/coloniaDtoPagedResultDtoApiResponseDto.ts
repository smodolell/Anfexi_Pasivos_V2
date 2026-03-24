import { ColoniaDtoPagedResultDto } from './coloniaDtoPagedResultDto';

export interface ColoniaDtoPagedResultDtoApiResponseDto {
    success?: boolean;
    message?: string | null;
    data?: ColoniaDtoPagedResultDto;
    errors?: Array<string> | null;
    statusCode?: number;
    timestamp?: string;
    traceId?: string | null;
}
