export class CfWidgetResponse {
    public id: number = 0;
    public title: string = "";
    public summary: string = "";
    public thumbnail: string = "";

    public urls?: CfWidgetUrlSet;
}

export class CfWidgetUrlSet {
    public curseforge: string = "";
    public project: string = "";
}