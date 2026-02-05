using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;

namespace Commands.Helpers
{
  public class AutoCompletionHandler : IAutoCompleteHandler
  {
    // 1. CRITICAL FIX: Do NOT include letters here. 
    // Only use characters that actually split words (Space, Slash, Backslash).
    public char[] Separators { get ; set ; } = [' ', '/', '\\'] ;

    private readonly string[] _commands ;
    private string _lastText = string.Empty ;
    private int _tabCount = 0 ;

    public AutoCompletionHandler( string[] commands )
    {
      _commands = commands ;
    }

    public string[] GetSuggestions( string text, int index )
    {
      // 2. State Sync: If user typed something new (or deleted), reset state.
      if ( text != _lastText ) {
        _tabCount = 0 ;
        _lastText = text ;
      }

      var matches = _commands.Where( c => c.StartsWith( text ) ).OrderBy( c => c ).ToArray() ;

      // Case A: No Matches -> Bell
      if ( matches.Length == 0 ) {
        _tabCount = 0 ; // Reset so we don't get stuck in a weird state
        Console.Write( '\a' ) ;
        return [] ;
      }

      // Case B: Exact Single Match -> Return Full String + Space
      if ( matches.Length == 1 ) {
        _tabCount = 0 ;
        // Return the FULL word. ReadLine replaces 'text' with this.
        return [matches[ 0 ] + " "] ;
      }

      // Case C: Multiple Matches -> Find Longest Common Prefix
      string commonPrefix = GetCommonPrefix( matches ) ;

      // If the shared prefix is longer than what we have...
      if ( commonPrefix.Length > text.Length ) {
        // 3. LOGIC FIX: 
        // We are about to update the text. We must set _tabCount to 1.
        // This tricks the shell into thinking "We just finished the 1st Tab action".
        // So if the user hits Tab again, it proceeds to the "List" action.
        _tabCount = 1 ;

        // Manually update _lastText so the next call doesn't reset _tabCount
        _lastText = commonPrefix ;

        // Return the FULL prefix. The library replaces the current text with this.
        return [commonPrefix] ;
      }

      // Case D: No Growth Possible -> Handle Bell vs List
      if ( _tabCount == 0 ) {
        // First Tab (and we can't grow): Ring Bell
        _tabCount++ ;
        Console.Write( '\a' ) ;
        return [] ;
      }

      // Second Tab: Show List
      Console.WriteLine() ;
      Console.WriteLine( string.Join( "  ", matches ) ) ;
      Console.Write( "$ " + text ) ; // Restore Prompt

      _tabCount = 0 ; // Reset
      return [] ;
    }

    private string GetCommonPrefix( string[] matches )
    {
      if ( matches.Length == 0 ) return "" ;
      string prefix = matches[ 0 ] ;

      foreach ( var s in matches.Skip( 1 ) ) {
        int i = 0 ;
        while ( i < prefix.Length && i < s.Length && prefix[ i ] == s[ i ] ) {
          i++ ;
        }

        prefix = prefix.Substring( 0, i ) ;
      }

      return prefix ;
    }
  }
}