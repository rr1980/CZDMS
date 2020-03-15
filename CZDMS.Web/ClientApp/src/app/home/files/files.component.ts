import { Component, OnInit } from '@angular/core';
import CustomFileProvider from 'devextreme/ui/file_manager/file_provider/custom';
import { PathInfo } from './file.items';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import notify from 'devextreme/ui/notify';
import { AuthService } from 'src/app/shared/services/auth.service';

@Component({
  selector: 'czdms-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {

  permissions: any = {
    create: true,
    copy: true,
    move: true,
    remove: true,
    rename: true,
    upload: true,
    download: true,
  };

  allowedFileExtensions: string[] = [".pdf"];
  fileProvider: CustomFileProvider;
  fileManagerComponent: any;

  contextMenu: any;
  toolbar: any;

  constructor(private http: HttpClient, private authService: AuthService) {
    this.onDownloadClick = this.onDownloadClick.bind(this);
  }

  ngOnInit() {
    this.setup();
  }

  onFileManagerInitialized(event) {
    this.fileManagerComponent = event.component;
  }

  onDownloadClick(e) {
    const selctedItems = this.fileManagerComponent.getSelectedItems() as any[];
    const isZip = selctedItems.length > 1 || selctedItems[0].isDirectory;
    const fileName = isZip ? selctedItems[0].name + ".zip" : selctedItems[0].name;

    const options = {
      headers: new HttpHeaders(
        {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + this.authService.getToken()
        }
      ),
      responseType: 'blob' as 'json'
    };

    const blob = this.http.post<Blob>('https://localhost:44351/api/DatabaseApi/Download', selctedItems, options).pipe(
      catchError((err) => {
        const msg = err.error?.message || "Error";
        notify(msg, 'error', 5000);
        return throwError({
          errorId: 0
        });
      })
    ).toPromise();

    blob.then(responseBlob => {
      if (navigator.appVersion.toString().indexOf('.NET') > 0) {
        window.navigator.msSaveBlob(responseBlob, fileName);
      }
      else {
        var url = URL.createObjectURL(responseBlob);
        var link = document.createElement("a");
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        var self = this;
        setTimeout(() => {
          document.body.removeChild(link);
          window.URL.revokeObjectURL(url);
        }, 1000);
      }
    }).catch(response => {
      console.log("catch", response);
    });
  }

  post_request(command: string, data: any, options: any = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) }) {
    return this.http.post('https://localhost:44351/api/DatabaseApi/' + command, data, options).pipe(
      catchError((err) => {
        const msg = err.error?.message || "Error";
        notify(msg, 'error', 5000);
        return throwError({
          errorId: 0
        });
      })
    );
  }

  setup() {
    this.contextMenu = {
      items: [
        // {
        //   text: 'Create',
        //   icon: 'newfolder',
        //   // onClick: (e)=>{
        //   //   console.debug("create");
        //   // }
        // },
        'create',
        'upload', 'rename', 'move', 'copy', 'delete', 'refresh',
        {
          text: 'Download',
          icon: 'download',
          onClick: this.onDownloadClick
        }
      ]
    };

    this.toolbar = {
      fileSelectionItems: [
        'move', 'copy', 'rename', 'separator', 'delete', 'refresh', 'clear',
        {
          widget: "dxButton",
          options: {
            text: "Download",
            icon: "download"
          },
          location: "before",
          onClick: this.onDownloadClick
        }
      ]
    };

    this.fileProvider = new CustomFileProvider({
      getItems: (pathInfo: PathInfo[]) => {
        return this.post_request('GetItems', pathInfo).toPromise();
      },
      renameItem: (item, name) => {
        return this.post_request('RenameItem', { parentDir: item, name }).toPromise();
      },
      createDirectory: (parentDir, name) => {
        return this.post_request('CreateDirectory', { parentDir, name }).toPromise();
      },
      deleteItem: (item) => {
        return this.post_request('DeleteItem', item).toPromise();
      },
      moveItem: (item, destinationDir) => {
        return this.post_request('MoveItem', { item, destinationDir }).toPromise();
      },
      copyItem: (item, destinationDir) => {
        return this.post_request('CopyItem', { item, destinationDir }).toPromise();
      },
      uploadFileChunk: (fileData, chunksInfo, destinationDir) => {
        const formData: FormData = new FormData();
        formData.append('file', fileData, fileData.name);
        formData.append('destinationDir', destinationDir.key.toString());

        return this.post_request('UploadFileChunk', formData, {}).toPromise();
      },
      abortFileUpload: (fileData, chunksInfo, destinationDir) => {
        console.debug('abortFileUpload', fileData, chunksInfo, destinationDir);
      },
      uploadChunkSize: 1048576000
    });
  }
}