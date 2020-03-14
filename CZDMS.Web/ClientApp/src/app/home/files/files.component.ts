import { Component, OnInit } from '@angular/core';
import CustomFileSystemProvider from 'devextreme/file_management/custom_provider';
import { fileItems, PathInfo } from './file.items';
// import RemoteFileProvider from 'devextreme/ui/file_manager/file_provider/remote';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import FileSystemItem from 'devextreme/file_management/file_system_item';
import UploadInfo from 'devextreme/file_management/upload_info';

@Component({
  selector: 'czdms-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {

  allowedFileExtensions: string[] = [".pdf"];
  fileProvider: CustomFileSystemProvider;
  constructor(private http: HttpClient, ) { }

  ngOnInit() {
    this.setup();
  }

  onSelectedFileOpened(event){
    console.debug('onSelectedFileOpened', event);
  }

  setup() {
    // this.fileProvider = new RemoteFileProvider({
    //   endpointUrl: "https://localhost:44351/api/DatabaseApi"
    // });


    this.fileProvider = new CustomFileSystemProvider({
      getItems: (fileSystemItem: FileSystemItem) => {
        console.debug('getItems', fileSystemItem);
        // return null;
        // if (!pathInfo.length) {
        //   pathInfo.push({
        //     key: "\\",
        //     name: null
        //   });
        // }

        return this.http.post('https://localhost:44351/api/DatabaseApi/GetItems', fileSystemItem, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('getItems', val)), catchError((err) => {
            console.debug(err);
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      renameItem: (item, name) => {
        console.debug('renameItem', item, name);
      },
      createDirectory: (parentDir: FileSystemItem, name: string) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/CreateDirectory', { parentDir, name }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('createDirectory', val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      deleteItem: (item: FileSystemItem) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/DeleteItem', item, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('deleteItem', val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      moveItem: (item: FileSystemItem, destinationDir: FileSystemItem) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/MoveItem', { item, destinationDir }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('moveItem', val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      copyItem: (item: FileSystemItem, destinationDir:FileSystemItem) => {
        return this.http.post('https://localhost:44351/api/DatabaseApi/CopyItem', { item, destinationDir }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
          .pipe(tap(val => console.debug('copyItem',val)), catchError((err) => {
            return throwError({
              errorId: 0
            });
          })).toPromise();
      },
      uploadFileChunk: (fileData: File, uploadInfo: UploadInfo) => {
        // const formData: FormData = new FormData();
        // formData.append('file', fileData, fileData.name);
        // formData.append('destinationDir', destinationDir.key.toString());

        return Promise.resolve(null);
        // return this.http.post('https://localhost:44351/api/DatabaseApi/UploadFileChunk', formData)
        //   .pipe(tap(val => console.debug('uploadFileChunk',val)), catchError((err) => {
        //     return throwError({
        //       errorId: 0
        //     });
        //   })).toPromise();
      },
      abortFileUpload: (fileData: File, uploadInfo: UploadInfo) => {
        console.debug('abortFileUpload', fileData, uploadInfo);
      },
      // uploadChunkSize: 1048576000
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
