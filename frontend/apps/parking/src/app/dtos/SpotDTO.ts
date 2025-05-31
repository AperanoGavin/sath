import { SpotCapability } from "./SpotCapability";

export interface SpotDTO {
    id: string;
    key: string;
    capabilities: SpotCapability[];
}
