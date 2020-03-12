import { Component } from '@angular/core';
import CustomFileProvider from 'devextreme/ui/file_manager/file_provider/custom';
import { fileItems, PathInfo } from './file.items';
import RemoteFileProvider from 'devextreme/ui/file_manager/file_provider/remote';

@Component({
  selector: 'czdms-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  allowedFileExtensions: string[] = [".pdf"];
  fileProvider: CustomFileProvider;

  constructor() {

    this.fileProvider = new RemoteFileProvider({
      endpointUrl: "https://localhost:44351/api/DatabaseApi"
    });

    // this.fileProvider = new CustomFileProvider({
    //   getItems: (pathInfo : PathInfo[]) => {
    //     console.debug('getItems', pathInfo);
    //     return pathInfo.length === 0 ? fileItems : this.getItems(pathInfo);
    //   },
    //   renameItem: (item, name) => {
    //     console.debug('renameItem', item, name);
    //   },
    //   createDirectory: (parentDir, name) => {
    //     console.debug('createDirectory', parentDir, name);
    //   },
    //   deleteItem: (item) => {
    //     console.debug('deleteItem', item);
    //   },
    //   moveItem: (item, destinationDir) => {
    //     console.debug('moveItem', item, destinationDir);
    //   },
    //   copyItem: (item, destinationDir) => {
    //     console.debug('copyItem', item, destinationDir);
    //   },
    //   uploadFileChunk: (fileData, chunksInfo, destinationDir) => {
    //     console.debug('uploadFileChunk', fileData, chunksInfo, destinationDir);
    //   },
    //   abortFileUpload: (fileData, chunksInfo, destinationDir) => {
    //     console.debug('abortFileUpload', fileData, chunksInfo, destinationDir);
    //   },
    //   uploadChunkSize: 1000
    // });
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
