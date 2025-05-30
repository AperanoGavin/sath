import { ReservationStatus } from "./ReservationStatus";

export interface ReservationDTO {
    id: string;
    spotId: string;
    userId: string;
    created_at: Date;
    from: Date;
    to: Date;
    status: ReservationStatus;
}
