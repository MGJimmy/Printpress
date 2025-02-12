import { ServiceCategoryEnum } from "./service-category.enum";

export interface ServiceGetDto {
    id: number;
    name: string;
    price: number;
    serviceCategory: ServiceCategoryEnum;
}