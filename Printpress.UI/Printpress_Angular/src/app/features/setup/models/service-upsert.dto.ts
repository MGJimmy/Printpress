import { ServiceCategoryEnum } from "./service-category.enum";

export interface ServiceUpsertDto {
    name: string;
    price: number;
    serviceCategory: ServiceCategoryEnum;
}