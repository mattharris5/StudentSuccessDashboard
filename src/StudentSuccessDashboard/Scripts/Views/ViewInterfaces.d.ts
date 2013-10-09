/// <reference path="../typings/jquery/jquery.d.ts" />

interface IManagePage {
    Initialize(): void;
    SetupControls(): void;
    SetupModals(): void;
}

interface ISelector {
    SelectedRows: string[];
    Remove(value: string): void;
    Add(value: string): void;
    Reset(): void;
    SetSelections(allAvailableRows: string[], selectAll: boolean): void;
    AddHiddenSelectedElements(): void;
    CheckSelectedElements(checkBoxes: JQuery): void;
    UpdateSelections(selectAll: boolean, filterCriteria: any): void;
}

interface IFilter {
    Initialize(datatable: IDataTable): void;
    PushToArray(aoData: any[]): void;
    ToJson(): any;
    Reset(): void;
}

interface IDataTable {
    DataTableElement: any;
    Initialize(filter: IFilter): void;
    Refresh(): void;
}