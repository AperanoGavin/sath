import { SpotCapability } from "./SpotCapability";

export interface SpotDTO {
    id: string;
    key: Date;
    capabilities: SpotCapability;
}
