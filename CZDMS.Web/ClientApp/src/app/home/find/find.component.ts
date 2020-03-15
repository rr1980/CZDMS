import { Component, OnInit, ViewChild } from '@angular/core';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import { AuthService } from 'src/app/shared/services/auth.service';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';

const typeCategories: string[] = [
  "File",
  "Folder",
  "Other"
];

@Component({
  selector: 'czdms-find',
  templateUrl: './find.component.html',
  styleUrls: ['./find.component.scss']
})
export class FindComponent implements OnInit {

  @ViewChild(DxDataGridComponent, { static: false }) dataGrid: DxDataGridComponent;

  dataSource: any;
  filterValue: Array<any>;
  popupPosition: any;
  typelookupData = {
    store: {
      type: 'array',
      data: typeCategories,
    },
    pageSize: 10,
    paginate: true
  };
  typeCategories = typeCategories;

  constructor(private authService: AuthService) {

    this.dataSource = createStore({
      key: "id",
      loadUrl: "api/DatabaseApi/Recherche",
      loadMethod: "GET",
      onBeforeSend: (operation, ajaxSettings) => {
        ajaxSettings.headers = {
          'Authorization': 'Bearer ' + this.authService.getToken()
        };
      }
    }),

    this.popupPosition = { of: window, at: "top", my: "top", offset: { y: 10 } };
    this.filterValue = [
      ['dateModified', '>=', new Date("3/1/2020")]
    ];
  }

  ngOnInit() {
  }
}


