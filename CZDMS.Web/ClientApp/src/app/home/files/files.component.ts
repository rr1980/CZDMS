import { Component, OnInit } from '@angular/core';
import CustomFileProvider from 'devextreme/ui/file_manager/file_provider/custom';
import { fileItems, PathInfo } from './file.items';
// import RemoteFileProvider from 'devextreme/ui/file_manager/file_provider/remote';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Component({
  selector: 'czdms-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {

  allowedFileExtensions: string[] = [".pdf"];
  fileProvider: CustomFileProvider;
  constructor(private http: HttpClient, ) { }

  ngOnInit() {
    this.setup();
  }


  setup() {
    // this.fileProvider = new RemoteFileProvider({
    //   endpointUrl: "https://localhost:44351/api/DatabaseApi"
    // });


    this.fileProvider = new CustomFileProvider({
      getItems: (pathInfo: PathInfo[]) => {
        console.debug('getItems', pathInfo);

        // if (!pathInfo.length) {
        //   pathInfo.push({
        //     key: "\\",
        //     name: null
        //   });
        // }

        return this.http.post('https://localhost:44351/api/DatabaseApi/GetItems', pathInfo, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          // .pipe(tap(val => console.debug('getItems', val)))
          .toPromise();
      },
      renameItem: (item, name) => {
        console.debug('renameItem', item, name);
      },
      createDirectory: (parentDir, name) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/CreateDirectory', { parentDir, name }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('createDirectory', val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      deleteItem: (item) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/DeleteItem', item, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('deleteItem', val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      moveItem: (item, destinationDir) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/MoveItem', { item, destinationDir }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('moveItem', val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      copyItem: (item, destinationDir) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/CopyItem', { item, destinationDir }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('copyItem',val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      uploadFileChunk: (fileData, chunksInfo, destinationDir) => {
        const formData: FormData = new FormData();
        formData.append('file', fileData, fileData.name);
        formData.append('destinationDir', destinationDir.key.toString());

        return this.http.post('https://localhost:44351/api/DatabaseApi/UploadFileChunk', formData)
          .pipe(tap(val => console.debug('uploadFileChunk',val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      abortFileUpload: (fileData, chunksInfo, destinationDir) => {
        console.debug('abortFileUpload', fileData, chunksInfo, destinationDir);
      },
      uploadChunkSize: 1048576000
    });
  }

  getItems(pathInfo: any[]) {
    const requestPathInfo = pathInfo[pathInfo.length - 1];
    const parts = requestPathInfo.key.split('/');

    let items = fileItems;
    for (let index = 0; index < parts.length; index++) {
      const part = parts[index];
      items = items.find(x => x.name === part).items;
    }

    return items;
  }
}
