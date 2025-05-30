export interface CreateReservationDTO {
    spotId: string;
    userId: string;
    from: Date;
    to: Date;
    isReservation: boolean;
}
