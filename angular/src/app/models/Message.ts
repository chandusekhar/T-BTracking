export interface Message{
    attachmentDto : []
    content : string
    conversationId: string
    creationTime : Date
    extraProperties : object
    id: string
    isGhim : string
    receiverName : string
    senderId: string
    senderName : string
    status: boolean
    type : number
    showDate : boolean
}