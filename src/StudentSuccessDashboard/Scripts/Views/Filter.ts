/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="ViewInterfaces.d.ts" />

class Filter implements IFilter {
    private _MultiOpenAccordion: any;

    constructor() {
        this._MultiOpenAccordion = $('#multiOpenAccordion');
    }

    Initialize(dataTable: IDataTable): void {
        throw new Error("Initialize is not implemented");
    }

    PushToArray(aoData: any[]): void {
        throw new Error("PushToArray is not implemented");
    }

    ToJson(): any {
        throw new Error("ToJson is not implemented");
    }
    
    Reset() {
        this._MultiOpenAccordion.multiOpenAccordion({
            active: 'none'
        });
        this._MultiOpenAccordion.multiOpenAccordion("option", "active", 'none');
    }
}