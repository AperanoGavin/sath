import { SpotCapability } from "./SpotCapability";

export interface CreateSpotDTO {
    key: string;
    capabilities: SpotCapability[];
}
