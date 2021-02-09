/* SYNTHSPACE AUDIO LAYER - https://synthspace.rocks
 * Copyright 2020 Bright Light Interstellar Limited - All rights reserved. */

using System.Collections.Generic;
using Songspace.Container;
using UnityEngine;

namespace Interaction.Audio {
    public class PatternContainer : ContainerGeneric<List<float>> {

        [SerializeField] private int m_CargoCount;
        [SerializeField] private string m_PatternString;

        public int CargoCount => m_CargoCount;

        public string PatternString {
            get {
                if( Cargo.Count == 0 ) return "EMPTY";
                if( !string.IsNullOrEmpty( m_PatternString ) ) return m_PatternString;
                string str = "";
                for( int i = 0; i < Cargo.Count; i++ ) {
                    str += Cargo[i].ToString( "0" ) + ( i < Cargo.Count - 1 ? ", " : "" );
                }
                return str;
            }
        }

        public override string GetLabel() {
            return PatternString;
        }

        private void Start() {
            m_CargoCount = Cargo.Count;
        }

        public void AddValue( float val ) {
            Cargo.Add( val );
            m_ContentChanged = true;
            m_CargoCount = Cargo.Count;
            //InvokeOnContentChanged();
        }

        public void Clear() {
            Cargo.Clear();
            m_ContentChanged = true;
            m_CargoCount = Cargo.Count;
            //InvokeOnContentChanged();
        }


        public override string SaveToFile() {
            //CheckPath( FolderPath );
            // if( string.IsNullOrEmpty( Filepath ) ) {
            //     Filepath = Path.Combine(StorageManager.i.ContainerInventoryAsset.GetStoragePath( Tag ), DateTime.Now.ToString( "yyyMMdd-HHmmss" ) + StorageManager.i.ContainerInventoryAsset.GetDefaultExtension(Tag));
            // }
            //
            // Debug.Log( "Save to File: " + Filepath );
            //
            // DataNode node = new DataNode( "Pattern", Filename );
            // node.AddChild( "Color", Color );
            // node.AddChild( "Name", PatternString );
            // node.AddChild( "Cargo", Cargo.ToArray() );
            //
            // if( !node.Write( Filepath, DataNode.SaveType.Text, false, false ) ) Debug.LogError( "Couldn't save" );
            return Filepath;
        }

        public override bool LoadFromFile( string filepath ) {
            Filepath = filepath;
            // DataNode node = null;
            //
            // if( File.Exists( Filepath ) ) {
            //     node = DataNode.Read( Filepath );
            // }
            //
            // // if( File.Exists( FilePath ) ) {
            // //     node = DataNode.Read( FilePath );
            // // }
            // // else if( File.Exists( StreamingAssetsFilePath ) ) {
            // //     node = DataNode.Read( StreamingAssetsFilePath );
            // // }
            //
            // if( node != null ) {
            //     Color = node.GetChild<Color>( "Color" );
            //     m_PatternString = node.GetChild<string>( "Name" );
            //     Cargo = new List<float>(node.GetChild<float[]>( "Cargo" ));
            //     m_CargoCount = Cargo.Count;
            //     return true;
            // }
            
            return false;
            //TODO: Handle StreamingAssets on Android (need to use UnityWebRequest to load)
        }
    }
}