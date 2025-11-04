export interface BTEmplInfoRequestModel {
    cardId: string;
    emplId: string;
}

export interface BTEmployeeInfoModel {
    emplId: string;
    emplName: string;
    mngrId: string;
    mngrName: string;
    phone: string;
}

export interface BTPerDiemRequestModel {
    cardId: string;
    cityRowId: string;
    dateFrom: string;
    dateTo: string; 
}

export interface BTPerDiemModel {
    days: number;
    total: number;
}
