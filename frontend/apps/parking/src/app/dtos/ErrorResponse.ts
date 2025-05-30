// https://datatracker.ietf.org/doc/html/rfc7807

import { HttpStatusCode } from "@angular/common/module.d-CnjH8Dlt";

export interface ErrorResponse {
    type: string;
    title: string;
    status: HttpStatusCode;
    detail: string;
}
