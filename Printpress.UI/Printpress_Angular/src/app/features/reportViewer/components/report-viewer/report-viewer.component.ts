import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ReportViewerService } from '../../services/report-viewer.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-report-viewer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './report-viewer.component.html',
  styleUrl: './report-viewer.component.css'
})
export class ReportViewerComponent implements OnInit {

  pdfUrl: SafeResourceUrl | null = null;

  constructor(private reportViewerService: ReportViewerService, private sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.reportViewerService.getReport().subscribe((blob :BlobPart ) => {
      debugger;
      const pdfBlob = new Blob([blob], { type: 'application/pdf' });
      const pdfUrl = URL.createObjectURL(pdfBlob);
      this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(pdfUrl);
    });
  }
}
